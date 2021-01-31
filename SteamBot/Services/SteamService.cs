using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
		
		public SteamService(SteamApiClient client, TelegramContext context)
		{
			_client = client;
			_context = context;
		}

		public async Task UpdateDb()
		{
			var market = await _client.GetCSGOItems();

			var skins = _context.Skins.ToList();
			foreach (var group in market.Items.GroupBy(a => new { SkinName = a.GetSkinName(), WeaponName = a.GetWeaponName() }))
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

					if (!first.IsFloated())
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
	}
}