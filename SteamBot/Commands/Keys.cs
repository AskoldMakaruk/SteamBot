using System;
using System.Collections.Generic;
using System.Linq;
using SteamBot.Localization;
using SteamBot.Model;
using Telegram.Bot.Types.ReplyMarkups;

namespace SteamBot.Commands
{
	public static class Keys
	{
		public static readonly string Tick = "✅";

		public static ReplyKeyboardMarkup ConfirmMarkup
		{
			get
			{
				ReplyKeyboardMarkup result = new[]
				{
					Texts.YesBtn
				};
				result.ResizeKeyboard = true;
				result.OneTimeKeyboard = true;
				return result;
			}
		}

		public static ReplyKeyboardMarkup GroupMenu
		{
			get
			{
				ReplyKeyboardMarkup result = new[]
				{
					"Cancel Trade",
					"Set price"
				};
				result.ResizeKeyboard = true;
				result.OneTimeKeyboard = true;
				return result;
			}
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

							var tick = selectedFloat != null && second == Helper.GetFloatName((float) selectedFloat) ? Tick : String.Empty;

							return new InlineKeyboardButton {CallbackData = $"{skin.Id} {second}", Text = $"{tick} {first}"};
						})
						.ToList())
					.ToList());
			}

			if (statTrak != null)
			{
				to.Add(new List<InlineKeyboardButton>
				{
					new()
					{
						CallbackData = $"{skin.Id} StatTrak",
						Text = $"{(statTrak == true ? Tick : string.Empty)} StatTrak"
					}
				});
			}

			to.Add(
				new List<InlineKeyboardButton>
				{
					new()
					{
						CallbackData = $"{skin.Id} Buy",
						Text = "Buy"
					},
					new()
					{
						CallbackData = $"{skin.Id} Sell",
						Text = "Sell"
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
					Text = "Buy",
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
					Text = "Buy",
					Url = inviteLink
				}
			};
		}
	}
}