using System;
using System.IO;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Middleware;
using SteamBot.Model;
using SteamBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamBot.Commands.AdminCommands
{
	public class ImportCsvCommand : StaticCommand
	{
		private readonly TranslationsService _translationsService;
		private readonly ITelegramBotClient _botClient;
		private readonly Account _account;

		public ImportCsvCommand(TranslationsService translationsService, AccountContext accountContext, ITelegramBotClient botClient)
		{
			_translationsService = translationsService;
			_botClient = botClient;
			_account = accountContext.Account;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Document != null;

		public override async Task Execute(IClient client)
		{
			if (!_account.IsAdmin)
			{
				return;
			}

			var message = (await client.GetUpdate()).Message;
			try
			{
				await using var stream = new MemoryStream();
				await _botClient.GetInfoAndDownloadFileAsync(message.Document.FileId, stream);

				stream.Seek(0, SeekOrigin.Begin);
				var count = _translationsService.ImportCsv(stream);
				_translationsService.ReloadTranslations();
				await client.SendTextMessage($"Done importing new translations.\nChanges count: {count}");
			}
			catch (Exception e)
			{
				await client.SendTextMessage($"Error importing file: {e}");
			}
		}
	}
}