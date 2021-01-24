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

			//todo price
			//todo re-enter float
			//todo image background
			//todo fix locale bug
			//todo 2 buttons with stattrak/no stattral
			ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;

			while (true)
			{
				var item = await FindItem();

				await using MemoryStream stream = new(item.Image.Bytes);

				var text = $"*{item.HashName}*\nPrice: {item.MarketPrice}$";
				await client.SendPhoto(new InputOnlineFile(stream, "item.png"), caption: text, parseMode: ParseMode.Markdown);
			}


			async Task<Item> FindItem()
			{
				await client.SendTextMessage(Texts.NewTradeText);
				var message = await client.GetTextMessage();
				var fl = 0f;
				while (true)
				{
					var items = _steamService.FindItems(message.Text).ToList();
					Item result = default;

					if (items.Count == 1)
					{
						var jsonItem = items[0];
						await client.SendTextMessage("Этот предмет искали?\n" + jsonItem.ToMarkupString(), replyMarkup: Keys.ConfirmMarkup, parseMode: ParseMode.Markdown);

						message = await client.GetTextMessage();

						if (message.Text == Texts.YesBtn)
						{
							result = await SelectFloat(jsonItem);
						}
					}
					else if (items.Count > 1)
					{
						ReplyKeyboardMarkup markup = items.Select(a => a.FullName).GroupElements(2).Select(a => a.ToArray()).ToArray();

						markup.ResizeKeyboard = true;
						markup.OneTimeKeyboard = true;
						await client.SendTextMessage("Выберите скин короче)", replyMarkup: markup);
						message = await client.GetTextMessage();

						var selected = items.First(a => a.FullName == message.Text);
						result = await SelectFloat(selected);
					}

					if (result != null)
					{
						return result;
					}

					await client.SendTextMessage("Ничего не найдено. Попробуйте еще раз");
					message = await client.GetTextMessage();
				}

				async Task<Item> SelectFloat(SteamService.JsonItem jsonItem)
				{
					await client.SendTextMessage(Texts.EnterFloatText, replyMarkup: Keys.FloatMarkup);

					while (true)
					{
						message = await client.GetTextMessage();

						if (Helper.TryGetFloatValue(message.Text, out fl, "ru-RU") || Single.TryParse(message.Text, NumberStyles.Any, Helper.Provider, out fl) && fl > 0 && fl <= 1)
						{
							var result = await _steamService.GetItem(jsonItem.FullName, fl);
							if (result == null)
							{
								await client.SendTextMessage("Нет предмета с таким качеством.");
								continue;
							}

							return result;
						}

						await client.SendTextMessage(Texts.EnterFloatText, replyMarkup: Keys.FloatMarkup);
					}
				}
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

		public static ReplyKeyboardMarkup FloatMarkup
		{
			get
			{
				ReplyKeyboardMarkup result = new[]
				{
					new[]
					{
						Texts.Float_Factory_New, Texts.Float_Minimal_Wear
					},
					new[]
					{
						Texts.Float_Field_Tested, Texts.Float_Well_Worn
					},
					new[]
					{
						Texts.Float_Battle_Scarred
					}
				};

				result.ResizeKeyboard = true;
				result.OneTimeKeyboard = true;

				return result;
			}
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