using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace SteamBot.Commands.AdminCommands
{
	public class UpdateDbCommand : StaticCommand
	{
		private readonly Database _context;
		private readonly SteamService _steamService;

		public UpdateDbCommand(SteamService steamService, Database context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/updatedb";

		public override async Task Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var acccount = _context.GetAccount(update.Message);

			if (acccount == null || !acccount.IsAdmin)
			{
				return;
			}

			var count = _context.Skins.Count();
			await client.SendTextMessage($"Skins count is {count}.");

			await _steamService.UpdateDb();
			count = _context.Skins.Count();
			await client.SendTextMessage($"New skins count is {count}.");
		}
	}

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

	public class DeleteOld : StaticCommand
	{
		private readonly Database _context;
		private readonly SteamService _steamService;

		public DeleteOld(SteamService steamService, Database context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/deleteold";

		public override async Task Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var acccount = _context.GetAccount(update.Message);

			if (acccount == null || !acccount.IsAdmin)
			{
				return;
			}

			var count = _context.Skins.Count();
			await client.SendTextMessage($"Skins count is {count}.");

			await _context.DeleteOldSkins();
			count = _context.Skins.Count();
			await client.SendTextMessage($"New skins count is {count}.");
		}
	}

	public class ExportCsv : StaticCommand
	{
		private readonly Database _context;
		private readonly TranslationsService _translationsService;

		public ExportCsv(Database context, TranslationsService translationsService)
		{
			_context = context;
			_translationsService = translationsService;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/exportcsv";

		public override async Task Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var acccount = _context.GetAccount(update.Message);

			if (acccount == null || !acccount.IsAdmin)
			{
				return;
			}

			var stream = _translationsService.ExportCsv();
			stream.Seek(0, SeekOrigin.Begin);
			await client.SendDocument(new InputOnlineFile(stream, $"{DateTime.Now.ToShortDateString()}.csv"));
			await stream.DisposeAsync();
		}
	}
}