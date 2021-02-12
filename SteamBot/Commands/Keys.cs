using System;
using System.Collections.Generic;
using System.Linq;
using SteamBot.Model;
using Telegram.Bot.Types.ReplyMarkups;
using static SteamBot.Services.TranslationsService;

namespace SteamBot.Commands
{
	public static class Keys
	{
		public const string Tick = "✅";

		public static ReplyKeyboardMarkup GroupMenu
		{
			get
			{
				ReplyKeyboardMarkup result = new[]
				{
					Locales["CancelTrade"],
					Locales["SetPrice"]
				};
				result.ResizeKeyboard = true;
				result.OneTimeKeyboard = true;
				return result;
			}
		}

		//todo locale
		public static IReplyMarkup StartKeys()
		{
			ReplyKeyboardMarkup startkeys = new[]
			{
				new[]
				{
					Locales["NewTradeBtn"],
					Locales["MyTradesBtn"]
				},
				new[]
				{
					Locales["MyFundsBtn"],
					Locales["MyStatsBtn"]
				}
			};
			startkeys.ResizeKeyboard = true;
			return startkeys;
		}

		public static InlineKeyboardMarkup FloatMarkup(Skin skin, string culture, float? selectedFloat, bool? statTrak = false)
		{
			var to = new List<List<InlineKeyboardButton>>();
			if (skin.IsFloated)
			{
				to.AddRange(skin.GetFloats(culture)
					.Zip(skin.GetFloats("en-EN"))
					.GroupElements(2)
					.Select(a => a.Select(c =>
						{
							var (first, second) = c;

							var tick = selectedFloat != null && second == Helper.GetFloatName((float)selectedFloat) ? Tick : String.Empty;

							return new InlineKeyboardButton { CallbackData = $"{skin.Id} {second}", Text = $"{tick} {first}" };
						})
						.ToList())
					.ToList());
			}

			if (statTrak != null && skin.StatTrakable)
			{
				to.Add(new List<InlineKeyboardButton>
				{
					new()
					{
						CallbackData = $"{skin.Id} StatTrak",
						Text = $"{(statTrak == true ? Tick : String.Empty)} StatTrak"
					}
				});
			}

			to.Add(
				new List<InlineKeyboardButton>
				{
					new()
					{
						CallbackData = $"{skin.Id} Buy",
						Text = Locales["BuyBtn"]
					},
					new()
					{
						CallbackData = $"{skin.Id} Sell",
						Text = Locales["SellBtn"]
					}
				});


			var result = new InlineKeyboardMarkup(to);
			return result;
		}

		public static InlineKeyboardMarkup ChannelMarkup(int id, string botName)
		{
			return new[]
			{
				new InlineKeyboardButton
				{
					CallbackData = $"{id} Buy",
					Text = Locales["BuyBtn"],
					Url = $"https://t.me/{botName}?start={id}"
				}
			};
		}

		public static InlineKeyboardMarkup GroupMarkup(string inviteLink)
		{
			return new[]
			{
				new InlineKeyboardButton
				{
					Text = Locales["BuyBtn"],
					Url = inviteLink
				}
			};
		}
	}
}