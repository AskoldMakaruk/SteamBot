using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Services;
using Telegram.Bot.Types;

namespace SteamBot.Commands.AdminCommands
{
	public class CloseAllTrades : StaticCommand
	{
		private readonly Database _context;

		public CloseAllTrades(Database context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/closetrades";

		public override async Task Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var acccount = _context.GetAccount(update.Message);

			if (acccount == null || !acccount.IsAdmin)
			{
				return;
			}

			var rooms = _context.ChatRooms.ToList();
			foreach (var room in rooms)
			{
				room.Trade.Room = null;
				room.Trade = null;
				room.TradeId = null;
			}

			await _context.SaveChangesAsync();
			await client.SendTextMessage($"Free rooms {rooms.Count}.");
		}
	}
}