using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Model;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using static SteamBot.Localization.Texts;

namespace SteamBot.Commands
{
	public class NewTradeCommand : StaticCommand
	{
		private readonly TelegramContext _context;
		private readonly SteamService _steamService;

		public NewTradeCommand(TelegramContext context, SteamService steamService)
		{
			_context = context;
			_steamService = steamService;
		}

		public override bool SuitableLast(Update message) => message?.Message?.Text != null; // == Texts.NewTradeBtn;

		public override async Task<Response> Execute(IClient client)
		{
			//todo 2 buttons with stattrak/no stattral
			//todo confirm inline btn
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


			var text = $"*{skin.SearchName}*\nPrice: {price}$";
			return await client.SendPhoto(new InputOnlineFile(stream, "skin.png"), caption: text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU"));
		}
	}

	public class ChannelCommand : StaticCommand
	{
		public override bool SuitableFirst(Update message) => message?.ChannelPost != null;

		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetUpdate();
			return default;
		}
	}

	public class SellSkinCommand : StaticCommand
	{
		private readonly SteamService _steamService;
		private readonly TelegramContext _context;

		public SellSkinCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.CallbackQuery?.Data.Contains("Sell") ?? false;

		public override async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var account = _context.GetAccount(query.From);
			var skin = await _context.Skins.FindAsync(int.Parse(query.Data.Split(' ')[0]));

			account.CurrentTrade = new TradeItem
			{
				Skin = skin,
			};

			await client.SendTextMessage(ResourceManager.GetString("EnterPrice"));

			account.CurrentTrade.Price = await client.GetValue<double>();

			await client.SendTextMessage(ResourceManager.GetString("TradeCreated"));


			//todo create Trade add trade item post to channel
			//account.Trades.Add(account.CurrentTrade);


			return new Response();
		}
	}

	public class FloatInlineCommand : StaticCommand
	{
		private readonly SteamService _steamService;
		private readonly TelegramContext _context;

		public FloatInlineCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => Helper.Floats().Any(a => message?.CallbackQuery?.Data.Contains(a) ?? false);

		public override async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var skin = await _context.Skins.FindAsync(int.Parse(query.Data.Split(' ')[0].Trim()));

			var floatString = query.Data[query.Data.IndexOf(' ')..];
			if (Helper.TryGetFloatValue(floatString, out var fl) || Single.TryParse(floatString, NumberStyles.Any, Helper.Provider, out fl) && fl > 0 && fl <= 1)
			{
				if (skin.GetMarketPrice(fl) == null)
				{
					await client.AnswerCallbackQuery("Нет предмета с таким качеством.");
				}

				var text = skin.ToMessage(fl);

				//todo check if message 
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

	public class UpdateDbCommand : StaticCommand
	{
		private readonly SteamService _steamService;
		private readonly TelegramContext _context;

		public UpdateDbCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/updatedb";

		public override async Task<Response> Execute(IClient client)
		{
			var count = _context.Skins.Count();
			await client.SendTextMessage($"Skins count is {count}.");
			var update = await client.GetTextMessage();
			var acccount = _context.GetAccount(update);
			if (acccount == null || !acccount.IsAdmin)
			{
				return new Response();
			}

			await _steamService.UpdateDb();
			count = _context.Skins.Count();
			await client.SendTextMessage($"New skins count is {count}.");

			return new Response();
		}
	}
}