using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
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

		public async Task<Item> GetItem(string name, float fl)
		{
			Console.WriteLine($"Trying to get item with name {name} ({Helper.GetFloatName(fl)})");

			var (hashName, price) = Bag.FirstOrDefault(a => a.Name.Contains(name.Trim()) && a.Name.Contains(Helper.GetFloatName(fl)));

			Console.WriteLine($"Hashname is {hashName}");

			var result = await GetByHashName(hashName);
			result.MarketPrice = price;
			return result;
		}

		public async Task<Item> GetByHashName(string hashName)
		{
			var item = await _client.GetCSGOItem(hashName);
			var img = new Image();
			using (var client = new WebClient())
			{
				img.Bytes = await client.DownloadDataTaskAsync(new Uri(item.Image));
				await _context.Images.AddAsync(img);
			}

			var marketHashName = item.MarketHashName;

			var isKnife = marketHashName.Contains(Helper.Star);
			if (isKnife)
			{
				marketHashName = marketHashName.Replace(Helper.Star, String.Empty).Trim();
			}

			var isStatTrak = marketHashName.Contains(Helper.StatTrak);
			if (isStatTrak)
			{
				marketHashName = marketHashName.Replace(Helper.StatTrak, String.Empty).Trim();
			}

			var delimiterIndx = marketHashName.IndexOf('|');

			var weapon = marketHashName[..delimiterIndx].Trim();
			var skinName = marketHashName[(delimiterIndx + 2)..marketHashName.IndexOf('(')].Trim();

			var result = new Item
			{
				Image = img,
				IsKnife = isKnife,
				IsStatTrak = isStatTrak,
				WeaponName = weapon,
				SkinName = skinName
			};
			return result;
		}

		private record JsonItem(string Name, double Price);
	}
}