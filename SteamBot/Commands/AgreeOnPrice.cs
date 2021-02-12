using System;
using System.Globalization;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using Microsoft.EntityFrameworkCore;
using SteamBot.Model;
using SteamBot.Services;
using Telegram.Bot.Types;
using static SteamBot.Services.TranslationsService;

namespace SteamBot.Commands
{
	public class AgreeOnPrice : StaticCommand
	{
		private readonly Database _context;

		public AgreeOnPrice(Database context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message.Message?.Text == Locales["SetPrice"];

		//todo redo with db save
		public override async Task Execute(IClient client)
		{
			var update = await client.GetTextMessage();
			var chatRoom = await _context.ChatRooms.FirstOrDefaultAsync(a => a.ChatId == update.Chat.Id);

			var trade = chatRoom.Trade;

			double sellersPrice = default, buyersPrice = default;
			await client.SendTextMessage(Locales["WaitingForPriceText"], chatRoom.ChatId);
			do
			{
				var msg = await client.GetTextMessage(a => a.From.Id == trade.Seller.ChatId || a.From.Id == trade.Buyer.ChatId);

				if (msg.From.Id == trade.Seller.ChatId)
				{
					if (Double.TryParse(msg.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out sellersPrice))
					{
						trade.SellerPrice = sellersPrice;
					}
				}

				if (msg.From.Id == trade.Buyer.ChatId)
				{
					if (Double.TryParse(msg.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out buyersPrice))
					{
						trade.BuyerPrice = buyersPrice;
					}
				}
			} while (Math.Abs(sellersPrice - buyersPrice) > 0.0001);

			trade.Status = TradeStatus.PaymentConfirmation; // todo idk


			await client.SendTextMessage(Locales["PriceSetText"], chatRoom.ChatId);
			await client.SendTextMessage(Locales["SendMoneyText"], chatRoom.ChatId);
			await _context.SaveChangesAsync();
		}
	}
}