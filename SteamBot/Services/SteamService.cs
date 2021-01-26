using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamApi;
using SteamApi.Model;
using SteamBot.Database;
using SteamBot.Model;

namespace SteamBot.Services
{
	public class SteamService
	{
		private readonly SteamApiClient _client;
		private readonly TelegramContext _context;
		private static ConcurrentBag<JsonItem> Bag;

		static SteamService()
		{
			var json = File.ReadAllText("compact.json");
			var jobject = JObject.Parse(json);
			var list = jobject.Children()
				.OfType<JProperty>()
				.Select(a => new JsonItem(a.Name, a.Value.Value<double>()))
				.ToList();

			Bag = new ConcurrentBag<JsonItem>(list);
		}

		public SteamService(SteamApiClient client, TelegramContext context)
		{
			_client = client;
			_context = context;
		}

		public async Task UpdateDb()
		{
			var skins = _context.Skins.ToList();
			foreach (var group in Bag.GroupBy(a => new {a.SkinName, a.WeaponName}))
			{
				try
				{
					var item = skins.FirstOrDefault(a => a.WeaponName == group.Key.WeaponName && a.SkinName == group.Key.SkinName);

					var first = group.First();
					if (item == null)
					{
						item = new Skin();
						item.ParseHashName(first.Name);
						await _context.Skins.AddAsync(item);
					}
					else
					{
						item.ParseHashName(first.Name);
					}

					if (!first.IsFloated)
					{
						item.Price = first.Price;
					}
					else
					{
						var battleScarred = group.FirstOrDefault(a => a.Name.Contains("Battle-Scarred"));
						var factoryNew = group.FirstOrDefault(a => a.Name.Contains("Factory New"));
						var fieldTested = group.FirstOrDefault(a => a.Name.Contains("Field-Tested"));
						var minimalWear = group.FirstOrDefault(a => a.Name.Contains("Minimal Wear"));
						var wellWorn = group.FirstOrDefault(a => a.Name.Contains("Well-Worn"));

						item.BattleScarredPrice = battleScarred?.Price;
						item.FactoryNewPrice = factoryNew?.Price;
						item.FieldTestedPrice = fieldTested?.Price;
						item.MinimalWearPrice = minimalWear?.Price;
						item.WellWornPrice = wellWorn?.Price;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(group.Key);
				}
			}

			await _context.SaveChangesAsync();
		}

		public IEnumerable<Skin> FindItems(string name)
		{
			return _context.Skins.Where(skin => skin.SearchName.ToLower().Contains(name.Trim().ToLower()));
		}

		//public async Task<TradeItem> GetItem(string name, float fl)
		//{
		//	Console.WriteLine($"Trying to get item with name {name} float: {fl} ({Helper.GetFloatName(fl)})");

		//	var jsonResult = FindItems(name).FirstOrDefault();

		//	if (jsonResult is null)
		//	{
		//		return null;
		//	}

		//	var hashName = jsonResult.GetHashName(fl);

		//	Console.WriteLine($"Hashname is {hashName}");

		//	var result = await GetSteamItem(hashName);
		//	result.Float = fl;
		//	return result;
		//}

		public async Task<Item> GetSteamItem(Skin skin, float fl)
		{
			Item item;

			if (skin.SteamItem != null)
			{
				item = JsonConvert.DeserializeObject<Item>(skin.SteamItem.Json);
			}
			else
			{
				item = await _client.GetCSGOItem(skin.GetHashName(fl));

				skin.SteamItem = new SteamItem
				{
					Json = JsonConvert.SerializeObject(item),
					Skin = skin,
				};
				await _context.SteamItems.AddAsync(skin.SteamItem);
			}

			//todo update price

			await _context.GetImage(skin, fl, item.Image);
			await _context.SaveChangesAsync();

			return item;
		}


		public class JsonItem
		{
			public string Name { get; set; }
			public double Price { get; set; }
			public bool IsFloated => Helper.IsFloated(Name);
			public bool HasDelimiter => Name.Contains('|');

			public bool IsKnife => Name.Contains(Helper.Star);
			public bool IsStatTrak => Name.Contains(Helper.StatTrak);

			public string NormalizedName => Name.Replace(Helper.Star, String.Empty).Replace(Helper.StatTrak, String.Empty).Trim();

			public string FullName => $"{WeaponName} | {SkinName}";

			public string WeaponName
			{
				get
				{
					return Skin.GetNormalizedName(Name).WeaponName;
					if (HasDelimiter)
					{
						var delimiterIndx = NormalizedName.IndexOf('|');
						return NormalizedName[..delimiterIndx].Trim();
					}

					return Name.Trim();
				}
			}

			public string SkinName
			{
				get
				{
					return Skin.GetNormalizedName(Name).SkinName;
					if (HasDelimiter)
					{
						var delimiterIndx = NormalizedName.IndexOf('|');

						return (IsFloated ? NormalizedName[(delimiterIndx + 2)..NormalizedName.IndexOf('(')] : NormalizedName[(delimiterIndx + 2)..]).Trim();
					}

					return WeaponName;
				}
			}


			public JsonItem(string name, double price)
			{
				Name = name;
				Price = price;
			}

			public void Deconstruct(out string hashname, out double price)
			{
				hashname = Name;
				price = Price;
			}

			public string ToMarkupString()
			{
				return $"*{Name}*\nPrice: {Price}$";
			}
		}
	}
}