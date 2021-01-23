using System;
using System.IO;
using System.Net.Mime;
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
			//todo name from json
			//todo float
			//todo price
			await client.SendTextMessage(Texts.NewTradeText);
			var message = await client.GetTextMessage();

			var account = _context.GetAccount(message);
			while (true)
			{
				var item = await _steamService.GetItem(message.Text, 0.16f);

				await using (MemoryStream stream = new(item.Image.Bytes))
				{
					var text = $"*{item.HashName}*\nPrice: {item.MarketPrice}$";
					await client.SendPhoto(new InputOnlineFile(stream, "item.png"), caption: text, parseMode: ParseMode.Markdown);
				}

				message = await client.GetTextMessage();
			}
			

			var trade = new Trade
			{
				Seller = account
			};
			return new Response();
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