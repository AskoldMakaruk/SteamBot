using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SteamApi;
using SteamApi.Model;
using SteamBot.Model;

namespace SteamBot.Services
{
	public class SteamService
	{
		private readonly SteamApiClient _client;
		private readonly Database _context;

		public SteamService(SteamApiClient client, Database context)
		{
			_client = client;
			_context = context;
		}

		public async Task UpdateDb()
		{
			var market = await _client.GetCSGOItems();

			var skins = _context.Skins.Include(a => a.Prices).ToList();
			foreach (var group in market.Items.GroupBy(a => new {SkinName = a.GetSkinName(), WeaponName = a.GetWeaponName()}))
			{
				try
				{
					var item = skins.FirstOrDefault(a => a.WeaponName == group.Key.WeaponName.Trim() && a.SkinName == group.Key.SkinName.Trim());

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

					item.IsStatTrak = group.Any(a => a.Name.Contains(Helper.StatTrak));

					foreach (var compact in group)
					{
						SkinPrice price;
						var statTrak = compact.Name.Contains(Helper.StatTrak);

						if (!compact.IsFloated())
						{
							price = item.Prices.FirstOrDefault(a => a.StatTrak == statTrak);

							if (price == null)
							{
								price = new SkinPrice();
								item.Prices.Add(price);
							}
						}
						else
						{
							var floatName = Helper.FloatsNames().First(name => compact.Name.Contains(name));

							price = item.Prices.FirstOrDefault(a => a.FloatName.Trim() == floatName.Trim() && a.StatTrak == statTrak);

							if (price == null)
							{
								price = new SkinPrice();
								item.Prices.Add(price);
							}

							price.Float = Helper.GetFloatValue(floatName);
							price.FloatName = floatName;
						}

						price.StatTrak = statTrak;
						price.Value = compact.Price;
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

		public async Task<Item> GetSteamItem(Skin skin, float? fl = null)
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
					Skin = skin
				};
				await _context.SteamItems.AddAsync(skin.SteamItem);
			}

			//todo update price

			await _context.GetImage(skin, fl, item.Image);
			await _context.SaveChangesAsync();

			return item;
		}
	}
}