using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using SteamBot.Services;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	//todo
	public class StatTrakInlineCommand : StaticCommand
	{
		private readonly Database _context;
		private readonly SteamService _steamService;

		public StatTrakInlineCommand(SteamService steamService, Database context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableLast(Update message) => message?.CallbackQuery?.Data.Contains("StatTrak") ?? false;

		public override async Task Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var data = query.Data.Split(' ');

			var skin = await _context.Skins.FindAsync(Int32.Parse(data[0]));

			var isStatTrak = !query.Message.IsStatTrak();

			var seletedFloat = query.Message.GetFloat();
			await client.UpdateSkin(query, skin, seletedFloat, isStatTrak);
		}
	}
}