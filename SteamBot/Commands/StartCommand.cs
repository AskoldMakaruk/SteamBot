using System;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Localization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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