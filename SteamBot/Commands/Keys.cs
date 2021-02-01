﻿using System.Linq;
using SteamBot.Localization;
using SteamBot.Model;
using Telegram.Bot.Types.ReplyMarkups;

namespace SteamBot.Commands
{
	public static class Keys
	{
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

		public static InlineKeyboardMarkup FloatMarkup(Skin skin, string culture)
		{
			if (!skin.IsFloated)
			{
				return null;
			}

			var to = skin.GetFloats(culture)
				.Zip(skin.GetFloats("en-EN"))
				.GroupElements(2)
				.Select(a => a.Select(c => new InlineKeyboardButton
					{
						CallbackData = $"{skin.Id} {c.Second}",
						Text = c.First
					})
					.ToList())
				.ToList();

			to.Add(new[]
			{
				//todo
				//new InlineKeyboardButton
				//{
				//	CallbackData = $"{skin.Id} StatTrak",
				//	Text = "StatTrak"
				//},
				new InlineKeyboardButton
				{
					CallbackData = $"{skin.Id} Buy",
					Text = "Buy"
				},
				new InlineKeyboardButton
				{
					CallbackData = $"{skin.Id} Sell",
					Text = "Sell"
				}
			}.ToList());
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
				},
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
				},
			};
		}

		public static InlineKeyboardMarkup ConfirmBuyer(long buyerId)
		{
			return new[]
			{
				new InlineKeyboardButton
				{
					CallbackData = $"{buyerId} Buyer",
					Text = "Accept trade",
				},
			};
		}
	}
}