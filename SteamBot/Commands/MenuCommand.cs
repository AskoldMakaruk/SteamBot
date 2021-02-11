﻿using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamBot.Commands
{
	public class MenuCommand : StaticCommand

	{
		public override bool SuitableFirst(Update message)
			=> message?.Message?.Text == "/menu";

		public override async Task Execute(IClient client)
		{
			var message = await client.GetTextMessage();
			if (message.Chat?.Type == ChatType.Group || message.Chat?.Type == ChatType.Supergroup)
			{
				await client.SendTextMessage("This is menu", replyMarkup: Keys.GroupMenu, chatId: message.Chat);
			}
		}
	}
}