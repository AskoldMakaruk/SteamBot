using System;
using System.IO;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace SteamBot.Commands.AdminCommands
{
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