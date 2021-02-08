using System;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Services;
using Telegram.Bot.Types;

namespace SteamBot.Commands.AdminCommands
{
	public class UpdateDbCommand : StaticCommand
	{
		private readonly TelegramContext _context;
		private readonly SteamService _steamService;

		public UpdateDbCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/updatedb";

		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var acccount = _context.GetAccount(update.Message);

			if (acccount == null || !acccount.IsAdmin)
			{
				return new Response();
			}

			var count = _context.Skins.Count();
			await client.SendTextMessage($"Skins count is {count}.");

			await _steamService.UpdateDb();
			count = _context.Skins.Count();
			await client.SendTextMessage($"New skins count is {count}.");

			return new Response();
		}
	}

	public class CloseAllTrades : StaticCommand
	{
		private readonly TelegramContext _context;

		public CloseAllTrades(TelegramContext context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/closetrades";

		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var acccount = _context.GetAccount(update.Message);

			if (acccount == null || !acccount.IsAdmin)
			{
				return new Response();
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

			return new Response();
		}
	}

	public class DeleteOld : StaticCommand
	{
		private readonly TelegramContext _context;
		private readonly SteamService _steamService;

		public DeleteOld(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/deleteold";

		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var acccount = _context.GetAccount(update.Message);

			if (acccount == null || !acccount.IsAdmin)
			{
				return new Response();
			}

			var count = _context.Skins.Count();
			await client.SendTextMessage($"Skins count is {count}.");

			await _context.DeleteOldSkins();
			count = _context.Skins.Count();
			await client.SendTextMessage($"New skins count is {count}.");

			return new Response();
		}
	}
}