using System;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using Microsoft.EntityFrameworkCore;
using SteamBot.Database;
using SteamBot.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetTextMessage();
			var chatRoom = _context.ChatRooms.FirstOrDefaultAsync(a => a.ChatId == update.Chat.Id);
			await client.GetTextMessage();

			return default;
		}
	}

	public class CancelTradeCommand : StaticCommand
	{
		private readonly TelegramContext _context;

		public CancelTradeCommand(TelegramContext context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message)
			=> message.Message?.Text == "Cancel Trade";

		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetUpdate();
			await client.SendTextMessage("Are you sure? Send this text if you're absolutely sure:\n\n```\nI'm absolutely sure.\n```", parseMode: ParseMode.Markdown, chatId: update.Message.Chat);
			await client.GetTradeCancelMessage();

			var chatroom = await _context.ChatRooms.FirstOrDefaultAsync(a => a.ChatId == update.Message.Chat.Id);

			var trade = chatroom.Trade;

			await client.KickChatMember((int) trade.Seller.ChatId, chatroom.ChatId);
			await client.KickChatMember((int) trade.Buyer.ChatId, chatroom.ChatId);

			trade.Buyer = null;
			trade.Room = null;
			trade.Status = TradeStatus.Open;

			chatroom.TradeId = null;

			await _context.SaveChangesAsync();
			//todo edit channelmessage
			//trade.ChannelPostId

			return default;
		}
	}


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

			var id = int.Parse(text);
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