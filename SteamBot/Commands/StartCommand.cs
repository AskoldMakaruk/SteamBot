using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Middleware;
using SteamBot.Model;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static SteamBot.Services.TranslationsService;

namespace SteamBot.Commands
{
	public class StartCommand : StaticCommand
	{
		private readonly Database _context;
		private readonly Account account;

		public StartCommand(Database context, AccountContext accountContext)
		{
			_context = context;
			account = accountContext.Account;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/start";

		public override async Task Execute(IClient client)
		{
			_ = await client.GetUpdate();
			if (account.TradeUrl is null)
			{
				await client.SendTextMessage(Locales["EN"]["EnterTradeUrlText"], disableWebPagePreview: true, parseMode: ParseMode.Markdown);
				var message = await client.GetTextMessage();

				while (!Uri.IsWellFormedUriString(message.Text, UriKind.Absolute))
				{
					await client.SendTextMessage(Locales["EN"]["EnterTradeUrlText"]);
					message = await client.GetTextMessage();
				}

				account.TradeUrl = message.Text;
				await _context.SaveChangesAsync();
			}

			await client.SendTextMessage(Locales["EN"]["StartText"], replyMarkup: Keys.StartKeys());
		}
	}
}