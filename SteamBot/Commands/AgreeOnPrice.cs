﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using Microsoft.EntityFrameworkCore;
using SteamBot.Database;
using SteamBot.Model;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	public class AgreeOnPrice : StaticCommand
	{
		private readonly TelegramContext _context;

		public AgreeOnPrice(TelegramContext context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message)
			=> message.Message?.Text == "Set price";

		//todo redo with db save
		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetTextMessage();
			var chatRoom = await _context.ChatRooms.FirstOrDefaultAsync(a => a.ChatId == update.Chat.Id);

			var trade = chatRoom.Trade;

			double sellersPrice = default, buyersPrice = default;
			await client.SendTextMessage("Waiting for both participants to send price: ", chatRoom.ChatId);
			do
			{
				var msg = await client.GetTextMessage(a => a.From.Id == trade.Seller.ChatId || a.From.Id == trade.Buyer.ChatId);

				if (msg.From.Id == trade.Seller.ChatId)
				{
					if (double.TryParse(msg.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out sellersPrice))
					{
						trade.SellerPrice = sellersPrice;
					}
				}

				if (msg.From.Id == trade.Buyer.ChatId)
				{
					if (double.TryParse(msg.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out buyersPrice))
					{
						trade.BuyerPrice = buyersPrice;
					}
				}
			} while (Math.Abs(sellersPrice - buyersPrice) > 0.0001);

			trade.Status = TradeStatus.PaymentConfirmation; // todo idk


			await client.SendTextMessage("Price set", chatRoom.ChatId);
			await client.SendTextMessage("Please send item of money yeah", chatRoom.ChatId);
			await _context.SaveChangesAsync();
			return default;
		}
	}
}