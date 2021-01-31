using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Services;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	public class UpdateDbCommand : StaticCommand
	{
		private readonly SteamService _steamService;
		private readonly TelegramContext _context;

		public UpdateDbCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => message?.Message?.Text == "/updatedb";

		public override async Task<Response> Execute(IClient client)
		{
			var count = _context.Skins.Count();
			await client.SendTextMessage($"Skins count is {count}.");
			var update = await client.GetTextMessage();
			var acccount = _context.GetAccount(update);
			if (acccount == null || !acccount.IsAdmin)
			{
				return new Response();
			}

			await _steamService.UpdateDb();
			count = _context.Skins.Count();
			await client.SendTextMessage($"New skins count is {count}.");

			return new Response();
		}
	}
}