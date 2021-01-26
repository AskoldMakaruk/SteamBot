using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Localization;
using SteamBot.Model;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace SteamBot.Commands
{
	public class NewTradeCommand : IStaticCommand
	{
		private readonly TelegramContext _context;
		private readonly SteamService _steamService;

		public NewTradeCommand(TelegramContext context, SteamService steamService)
		{
			_context = context;
			_steamService = steamService;
		}

		public bool SuitableFirst(Update message) => message?.Message?.Text == Texts.NewTradeBtn;

		public async Task<Response> Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var account = _context.GetAccount(update.Message);

			//todo fix float for !IsFloated

			//todo float from inline
			//todo 2 buttons with stattrak/no stattral
			//todo price

			//todo image background

			//todo fix locale bug
			//fuck i need to migrate Texts."Key" -> ResourceManager.GetString("Key", culture)


			//todo getInline
			while (true)
			{
				await FindItem();
			}


			async Task FindItem()
			{
				await client.SendTextMessage(Texts.NewTradeText);
				var message = await client.GetTextMessage();

				while (true)
				{
					var skins = _steamService.FindItems(message.Text).ToList();
					//TradeItem result = default;

					if (skins.Count == 1)
					{
						var skin = skins[0];
						await SendItem(skin);
						message = await client.GetTextMessage();
						continue;
						await client.SendTextMessage("Этот предмет искали?\n" + skin.ToMarkupString(), replyMarkup: Keys.ConfirmMarkup, parseMode: ParseMode.Markdown);

						

						if (message.Text == Texts.YesBtn)
						{
							await SelectFloat(skin);
							continue;
						}
					}
					else if (skins.Count > 1)
					{
						ReplyKeyboardMarkup markup = skins.Select(a => a.SearchName).GroupElements(2).Select(a => a.ToArray()).ToArray();

						markup.ResizeKeyboard = true;
						markup.OneTimeKeyboard = true;
						await client.SendTextMessage("Выберите скин короче)", replyMarkup: markup);
						message = await client.GetTextMessage();

						//var selected = skins.First(a => a.SearchName == message.Text);
						//await SelectFloat(selected);
						continue;
					}

					await client.SendTextMessage("Ничего не найдено. Попробуйте еще раз");
					message = await client.GetTextMessage();
				}
			}

			async Task SelectFloat(Skin skin)
			{
				//if (!skin.IsFloated)
				{
					await SendItem(skin);
					return;
				}

				//await client.SendTextMessage(Texts.EnterFloatText, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));

				//while (true)
				//{
				//	var message = await client.GetTextMessage();

				//	if (Helper.TryGetFloatValue(message.Text, out var fl, "ru-RU") || Single.TryParse(message.Text, NumberStyles.Any, Helper.Provider, out fl) && fl > 0 && fl <= 1)
				//	{
				//		if (skin.GetMarketPrice(fl) == null)
				//		{
				//			await client.SendTextMessage("Нет предмета с таким качеством.");
				//			continue;
				//		}

				//		await SendItem(skin, fl);
				//	}

				//	await client.SendTextMessage(Texts.EnterFloatText, replyMarkup: );
				//}
			}

			async Task SendItem(Skin skin, float fl = default)
			{
				string price = "";
				if (fl == 0)
				{
					var prices = skin.GetPrices();
					price = $"{prices.Min():0.##}$ - {prices.Max():0.##}$";
				}
				else
				{
					price = $"{skin.GetMarketPrice(fl)}$";
				}

				if (skin.IsFloated && fl == 0)
				{
					Helper.TryGetFloatValue(skin.GetFloats("en-EN").First(), out fl);
				}
				var image = await _context.GetImage(skin, fl);
				if (image == null)
				{
					await _steamService.GetSteamItem(skin, fl);
					image = skin.GetImage(fl);
				}

				await using MemoryStream stream = new(image.Bytes);

				
				var text = $"*{skin.SearchName}*\nPrice: {price}";
				await client.SendPhoto(new InputOnlineFile(stream, "skin.png"), caption: text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));
			}

			var trade = new Trade
			{
				Seller = account
			};
			return new Response();
		}
	}

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

		public static IReplyMarkup FloatMarkup(Skin skin, string culture)
		{
			if (!skin.IsFloated)
			{
				return null;
			}

			var result = new InlineKeyboardMarkup(skin.GetFloats(culture)
				.GroupElements(2)
				.Select(a => a.Select(c => new InlineKeyboardButton
					{
						CallbackData = $"{skin.Id} {c}",
						Text = c
					})
				));

			return result;
		}
	}


	public class StartCommand : IStaticCommand
	{
		private readonly TelegramContext _context;

		public StartCommand(TelegramContext context)
		{
			_context = context;
		}

		public bool SuitableFirst(Update message) => message?.Message?.Text == "/start";

		public async Task<Response> Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var account = _context.GetAccount(update.Message);
			if (account.TradeUrl is null)
			{
				await client.SendTextMessage(Texts.EnterTradeUrlText, disableWebPagePreview: true, parseMode: ParseMode.Markdown);
				var message = await client.GetTextMessage();

				while (!Uri.IsWellFormedUriString(message.Text, UriKind.Absolute))
				{
					await client.SendTextMessage(Texts.EnterUrlText);
					message = await client.GetTextMessage();
				}

				account.TradeUrl = message.Text;
				await _context.SaveChangesAsync();
			}

			ReplyKeyboardMarkup startkeys = new[]
			{
				new[]
				{
					Texts.NewTradeBtn,
					Texts.MyTradesBtn
				},
				new[]
				{
					Texts.MyMoneyBtn,
					Texts.MyStats
				}
			};
			startkeys.ResizeKeyboard = true;

			await client.SendTextMessage(Texts.StartText, replyMarkup: startkeys);

			return new Response();
		}
	}
}