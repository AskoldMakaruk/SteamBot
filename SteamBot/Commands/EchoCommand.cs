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
			//todo 2 buttons with stattrak/no stattral
			//todo price
			//todo image background
			//fuck i need to migrate Texts."Key" -> ResourceManager.GetString("Key", culture)

			var message = await client.GetTextMessage();
			while (true)
			{
				var skins = _steamService.FindItems(message.Text).ToList();

				if (skins.Count == 1)
				{
					var skin = skins[0];
					await SendSkin(client, skin);
				}
				else if (skins.Count > 1)
				{
					ReplyKeyboardMarkup markup = skins.Select(a => a.SearchName).GroupElements(2).Select(a => a.ToArray()).ToArray();

					markup.ResizeKeyboard = true;
					markup.OneTimeKeyboard = true;
					await client.SendTextMessage("Выберите скин короче)", replyMarkup: markup);
				}
				else
				{
					await client.SendTextMessage("Ничего не найдено. Попробуйте еще раз");
				}

				message = await client.GetTextMessage();
			}


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
					await client.AnswerCallbackQuery("Нет предмета с таким качеством.");
				}

				var price = skin.GetMarketPrice(fl)?.ToString("F", CultureInfo.InvariantCulture);
				var text = $"*{skin.SearchName}*\n{Helper.GetFloatName(fl)}\nPrice: {price}";

				//todo edit image?
				if (query.InlineMessageId != null)
				{
					await client.EditMessageCaption(query.InlineMessageId, text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));
				}
				else
				{
					await client.EditMessageCaption(query.Message.MessageId, text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));
				}
			}

			return new Response();
		}
	}
}