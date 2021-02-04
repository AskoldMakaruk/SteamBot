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
}