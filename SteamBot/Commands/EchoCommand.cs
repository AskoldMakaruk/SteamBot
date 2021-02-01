using System;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Localization;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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

		public override bool SuitableLast(Update message) => message?.Message?.Text == Texts.NewTradeBtn;

		public override async Task<Response> Execute(IClient client)
		{
			//todo 2 buttons with stattrak/no stattral
			//todo image background
			//fuck i need to migrate Texts."Key" -> ResourceManager.GetString("Key", culture)

			var _ = await client.GetUpdate();
			await client.SendTextMessage("Send item name to start:");
			var message = await client.GetTextMessage();
			while (true)
			{
				var skins = _steamService.FindItems(message.Text).ToList();

				if (skins.Count == 1)
				{
					var skin = skins[0];
					var fl = skin.Prices.OrderBy(a => a.Float).First().Float;

					if (skin.GetImage(fl) == null)
					{
						await _steamService.GetSteamItem(skin, fl);
						skin = await _context.Skins.FindAsync(skin.Id);
					}

					await client.SendSkin(skin, fl: fl ?? default);
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
	}

	//public class ChannelCommand : StaticCommand
	//{
	//	public override bool SuitableFirst(Update message) => message?.ChannelPost != null;

	//	public override async Task<Response> Execute(IClient client)
	//	{
	//		var update = await client.GetUpdate();
	//		Console.WriteLine(update.ChannelPost.Chat.Id);
	//		await client.SendTextMessage("Message from bot", update.ChannelPost.Chat.Id);
	//		return default;
	//	}
	//}

	public class FloatInlineCommand : StaticCommand
	{
		private readonly SteamService _steamService;
		private readonly TelegramContext _context;

		public FloatInlineCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => Helper.FloatsNames().Any(a => message?.CallbackQuery?.Data.Contains(a) ?? false);

		public override async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var skin = await _context.GetSkinAsync(query);

			var flN = query.GetFloat();
			if (flN > 0 && flN <= 1)
			{
				var fl = (float) flN;
				if (skin.GetPrice(fl) == null)
				{
					await client.AnswerCallbackQuery("Нет предмета с таким качеством.");
				}

				var text = skin.ToMessage(fl: fl);

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

	
}