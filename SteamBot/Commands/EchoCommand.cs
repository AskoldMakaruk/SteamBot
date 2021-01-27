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
using Npgsql.Replication;
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

		public bool SuitableLast(Update message) => message?.Message?.Text != null; // == Texts.NewTradeBtn;

		public async Task<Response> Execute(IClient client)
		{
			//var update = await client.GetUpdate();
			//var account = _context.GetAccount(update.Message);

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
				//await client.SendTextMessage(Texts.NewTradeText);
				var message = await client.GetTextMessage();

				while (true)
				{
					var skins = _steamService.FindItems(message.Text).ToList();
					//TradeItem result = default;

					if (skins.Count == 1)
					{
						var skin = skins[0];
						await SendSkin(client, skin);
						message = await client.GetTextMessage();
						continue;
						//await client.SendTextMessage("Этот предмет искали?\n" + skin.ToMarkupString(), replyMarkup: Keys.ConfirmMarkup, parseMode: ParseMode.Markdown);


						//if (message.Text == Texts.YesBtn)
						//{
						//	await SelectFloat(skin);
						//	continue;
						//}
					}

					if (skins.Count > 1)
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
				//{
				//	await SendItem(skin);
				//	return;
				//}

				//await client.SendTextMessage(Texts.EnterFloatText, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));

				//while (true)
				//{
				//	var message = await client.GetTextMessage();
			}

			//var trade = new Trade
			//{
			//	Seller = account
			//};
			return new Response();
		}

		public async Task<Message> SendSkin(IClient client, Skin skin, float fl = default)
		{
			string price = "";
			if (fl == 0)
			{
				var prices = skin.GetPrices();
				price = $"{prices.Min().ToString("F", CultureInfo.InvariantCulture)}$ - {prices.Max().ToString("F", CultureInfo.InvariantCulture)}$";
			}
			else
			{
				price = $"{skin.GetMarketPrice(fl)?.ToString("C2", CultureInfo.InvariantCulture)}$";
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
			return await client.SendPhoto(new InputOnlineFile(stream, "skin.png"), caption: text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));
		}
	}


	//todo
	public class FloatInlineCommand : IStaticCommand
	{
		private readonly SteamService _steamService;

		public FloatInlineCommand(SteamService steamService)
		{
			_steamService = steamService;
		}

		public bool SuitableLast(Update message) => Helper.Floats().Any(a => message?.CallbackQuery?.Data.Contains(a) ?? false);

		public async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var skin = _steamService.FindItems(query.Message.Caption.Split('\n')[0]).First();

			var floatString = query.Data[query.Data.IndexOf(' ')..];
			if (Helper.TryGetFloatValue(floatString, out var fl) || Single.TryParse(floatString, NumberStyles.Any, Helper.Provider, out fl) && fl > 0 && fl <= 1)
			{
				if (skin.GetMarketPrice(fl) == null)
				{
					await client.SendTextMessage("Нет предмета с таким качеством.");
				}

				var price = skin.GetMarketPrice(fl)?.ToString("F", CultureInfo.InvariantCulture);
				var text = $"*{skin.SearchName}*\n{Helper.GetFloatName(fl)}\nPrice: {price}";

				//todo edit message?
				if (query.InlineMessageId != null)
				{
					await client.EditMessageCaption(query.InlineMessageId, text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));
				}
				else
				{
					await client.EditMessageCaption(query.Message.MessageId, text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));
				}
			}

			//await client.SendTextMessage(Texts.EnterFloatText, replyMarkup:);
			return new Response();
		}
	}
}