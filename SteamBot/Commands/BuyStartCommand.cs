using System;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using Microsoft.EntityFrameworkCore;
using SteamBot.Database;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	public class BuyStartCommand : StaticCommand
	{
		private readonly TelegramContext _context;

		public BuyStartCommand(TelegramContext context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message)
			=> (message.Message?.Text?.StartsWith("/start") ?? false) && message.Message?.Text?.Length > "/start".Length;
		
		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetTextMessage();
			var buyer = _context.GetAccount(update);
			var text = update.Text.Replace("/start", String.Empty).Trim();

			var id = Int32.Parse(text);
			var trade = await _context.Trades.FindAsync(id);

			if (trade.Seller.Id == buyer.Id)
			{
				await client.SendTextMessage("Very funny...");
				return default;
			}

			if (trade.Buyer != null)
			{
				await client.SendTextMessage("This trade is already in progress. Soryy");
				return default;
			}

			var freeRoom = await _context.ChatRooms.FirstOrDefaultAsync(a => a.TradeId == null);
			if (freeRoom == null)
			{
				await client.SendTextMessage("No free rooms");
				//todo red flag for ilya
				return default;
			}

			trade.Room = freeRoom;
			trade.Buyer = buyer;
			await _context.SaveChangesAsync();

			foreach (var userChat in new[] {client.UserId, trade.Seller.ChatId})
			{
				await client.SendTextMessage("Please, join this chat room to proceed:", userChat, replyMarkup: Keys.GroupMarkup(freeRoom.InviteLink));
			}

			//await client.SendTextMessage($"Someone wants to buy {trade.TradeItem.Skin.SearchName}. Do you accept?", replyMarkup: Keys.ConfirmBuyer(update.From.Id), chatId: trade.Seller.ChatId);
			//await client.SendTextMessage("We notified seller about you. Wait for his reply, please.");

			return default;
		}
	}
}