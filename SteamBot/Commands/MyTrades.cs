using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using Microsoft.EntityFrameworkCore;
using SteamBot.Localization;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static SteamBot.Services.TranslationsService;

namespace SteamBot.Commands
{
	public class MyTrades : StaticCommand
	{
		private readonly Database _context;

		public MyTrades(Database context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == Locales["MyTradesBtn"];

		public override async Task Execute(IClient client)
		{
			var update = await client.GetTextMessage();
			var account = _context.GetAccount(update);

			var trades = _context.Trades
				.Include(a => a.Skin)
				.ThenInclude(a => a.Prices)
				.Where(a => a.Seller.Id == account.Id || a.Buyer.Id == account.Id)
				.ToList();

			var builder = new StringBuilder();
			for (var i = 0; i < trades.Count; i++)
			{
				var trade = trades[i];
				var skin = trade.Skin;
				var roomLink = trade.Room != null ? $"[Trade chat]({trade.Room.InviteLink})\n" : "";

				builder.AppendFormat(Locales["ListTradeItem"], i + 1, skin.SearchName, trade.StartPrice, trade.Status, roomLink);
				//builder.AppendFormat("{0}. *{1}*\nFor: ${2}\nStatus: {3}\n{4}\n", i + 1, skin.SearchName, trade.StartPrice, trade.Status, roomLink);
			}

			var text = builder.ToString();
			if (String.IsNullOrEmpty(text))
			{
				text = Locales["NoCurrentTradesText"];
				//text = "You don't have current trades.";
			}

			await client.SendTextMessage(text, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
		}
	}
}