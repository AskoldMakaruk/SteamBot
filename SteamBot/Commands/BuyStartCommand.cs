using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using Microsoft.EntityFrameworkCore;
using SteamBot.Services;
using Telegram.Bot.Types;
using static SteamBot.Services.TranslationsService;

namespace SteamBot.Commands
{
	public class BuyStartCommand : StaticCommand
	{
		private readonly Database _context;

		public BuyStartCommand(Database context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message) => (message.Message?.Text?.StartsWith("/start") ?? false) && message.Message?.Text?.Length > "/start".Length;

		public override async Task Execute(IClient client)
		{
			var update = await client.GetTextMessage();
			var buyer = _context.GetAccount(update);
			var text = update.Text.Replace("/start", String.Empty).Trim();

			var id = Int32.Parse(text);
			var trade = await _context.Trades.FindAsync(id);

			if (trade.Seller.Id == buyer.Id)
			{
				await client.SendTextMessage(Locales["SellerTryingToBuyHisItemError"]);
				return;
			}

			if (trade.Buyer != null)
			{
				await client.SendTextMessage(Locales["TradeInProgressError"]);
				return;
			}

			var freeRoom = await _context.ChatRooms.FirstOrDefaultAsync(a => a.TradeId == null);
			if (freeRoom == null)
			{
				await client.SendTextMessage(Locales["NoFreeChatRoomsError"]);
				//todo red flag for ilya
				return;
			}

			freeRoom.InviteLink = await client.ExportChatInviteLink(freeRoom.ChatId);

			trade.Room = freeRoom;
			trade.Buyer = buyer;
			await _context.SaveChangesAsync();

			foreach (var userChat in new[] {client.UserId, trade.Seller.ChatId})
			{
				await client.UnbanChatMember((int) userChat, freeRoom.ChatId);
				await client.SendTextMessage(Locales["JoinChatRoomText"], userChat, replyMarkup: Keys.GroupMarkup(freeRoom.InviteLink));
			}

			//await client.SendTextMessage($"Someone wants to buy {trade.TradeItem.Skin.SearchName}. Do you accept?", replyMarkup: Keys.ConfirmBuyer(update.From.Id), chatId: trade.Seller.ChatId);
			//await client.SendTextMessage("We notified seller about you. Wait for his reply, please.");
		}
	}
}