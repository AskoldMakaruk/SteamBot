﻿using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Services;
using Telegram.Bot.Types;

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
}