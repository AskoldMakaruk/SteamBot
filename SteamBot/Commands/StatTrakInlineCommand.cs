using System;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamBot.Commands
{
	//todo
	public class StatTrakInlineCommand : StaticCommand
	{
		private readonly TelegramContext _context;
		private readonly SteamService _steamService;

		public StatTrakInlineCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableLast(Update message) => message?.CallbackQuery?.Data.Contains("StatTrak") ?? false;

		public override async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var data = query.Data.Split(' ');

			var skin = await _context.Skins.FindAsync(Int32.Parse(data[0]));

			var isStatTrak = !query.Message.IsStatTrak();

			var seletedFloat = query.Message.GetFloat();
			await client.UpdateSkin(query, skin, seletedFloat, isStatTrak);

			return new Response();
		}
	}
}