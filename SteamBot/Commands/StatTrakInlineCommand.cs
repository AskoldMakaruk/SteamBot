using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Services;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	//todo
	public class StatTrakInlineCommand : StaticCommand
	{
		private readonly SteamService _steamService;
		private readonly TelegramContext _context;

		public StatTrakInlineCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;

		}
		public override bool SuitableLast(Update message) => message?.CallbackQuery?.Data == "StatTrak";

		public override async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var data = query.Data.Split('\n');

			var skin = await _context.Skins.FindAsync(int.Parse(data[0]));

			return new Response();
		}
	}
}