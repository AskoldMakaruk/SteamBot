using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SteamApi;
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

		public IEnumerable<JsonItem> FindItems(string name)
		{
			bool Compare(JsonItem a)
			{
				var hashName = a.Name.ToLower().Trim();
				return hashName.Contains(name.ToLower().Trim());
			}

			return Bag.Where(Compare).DistinctBy(a => new {a.WeaponName, a.SkinName});
		}

		public JsonItem FindItem(string name, float fl)
		{
			bool Compare(JsonItem a)
			{
				var hashName = a.Name.ToLower().Trim();
				var result = hashName.Contains(name.ToLower().Trim()) && hashName.Contains(Helper.GetFloatName(fl).ToLower().Trim());
				if (result)
				{
					Console.WriteLine(hashName);
					
				}

				return result;
			}

			return Bag.FirstOrDefault(Compare);
		}

		public async Task<Item> GetItem(string name, float fl)
		{
			Console.WriteLine($"Trying to get item with name {name} float: {fl} ({Helper.GetFloatName(fl)})");

			var jsonResult = FindItem(name, fl);

			if (jsonResult is null)
			{
				return null;
			}

			var (hashName, price) = jsonResult;

			Console.WriteLine($"Hashname is {hashName}");

			var result = await GetByHashName(hashName);
			result.MarketPrice = price;
			result.Float = fl;
			return result;
		}

		public async Task<Item> GetByHashName(string hashName)
		{
			var item = await _client.GetCSGOItem(hashName);
			var img = new Image();
			using (var client = new WebClient())
			{
				img.Bytes = await client.DownloadDataTaskAsync(new Uri(item.Image));
			}

			var result = new Item
			{
				Image = img
			};
			result.ParseHashName(hashName);
			return result;
		}

		public class JsonItem
		{
			public string Name { get; set; }
			public double Price { get; set; }

			public bool IsKnife => Name.Contains(Helper.Star);
			public bool IsStatTrak => Name.Contains(Helper.StatTrak);

			public string NormalizedName => Name.Replace(Helper.Star, String.Empty).Replace(Helper.StatTrak, String.Empty).Trim();

			public string FullName => $"{WeaponName} | {SkinName}";

			public string WeaponName
			{
				get
				{
					var delimiterIndx = NormalizedName.IndexOf('|');
					return NormalizedName[..delimiterIndx].Trim();
				}
			}

			public string SkinName
			{
				get
				{
					var delimiterIndx = NormalizedName.IndexOf('|');
					return NormalizedName[(delimiterIndx + 2)..NormalizedName.IndexOf('(')].Trim();
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