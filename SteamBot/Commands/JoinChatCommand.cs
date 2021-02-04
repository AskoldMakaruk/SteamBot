﻿using System;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using Microsoft.EntityFrameworkCore;
using SteamBot.Database;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	public class JoinChatCommand : StaticCommand
	{
		private readonly TelegramContext _context;

		public JoinChatCommand(TelegramContext context)
		{
			_context = context;
		}


		public override bool SuitableFirst(Update message) => message.Message?.NewChatMembers?.Length > 0;

		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var chat = update.Message.Chat;
			var chatRoom = await _context.ChatRooms.FirstOrDefaultAsync(a => a.ChatId == chat.Id);

			//ignore uninitialized chats
			if (chatRoom == null)
			{
				return default;
			}

			try
			{
				var a = await client.GetChatMember((int) chatRoom.Trade.Seller.ChatId, chatRoom.ChatId);
				var b = await client.GetChatMember((int) chatRoom.Trade.Buyer.ChatId, chatRoom.ChatId);

				chatRoom.AllMembersInside = true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			chatRoom.LastMemberChange = DateTime.Now;

			await _context.SaveChangesAsync();
			await client.SendTextMessage("Start trading guys..", chat);


			return default;
		}
	}
}