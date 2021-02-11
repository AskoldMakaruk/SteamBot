using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SteamBot.Services;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	public class JoinChatCommand : StaticCommand
	{
		private readonly Database _context;
		private readonly long ChannelId;

		public JoinChatCommand(Database context, IConfiguration configuration)
		{
			_context = context;
			ChannelId = Int64.Parse(configuration["ChannelId"]);
		}


		public override bool SuitableFirst(Update message) => message.Message?.NewChatMembers?.Length > 0;

		public override async Task Execute(IClient client)
		{
			var update = await client.GetUpdate();
			var chat = update.Message.Chat;
			var chatRoom = await _context.ChatRooms.FirstOrDefaultAsync(a => a.ChatId == chat.Id);

			//ignore uninitialized chats
			if (chatRoom == null)
			{
				return;
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

			try
			{
				//todo client channel edit message
				await client.ForwardMessage(ChannelId, (int) chatRoom.Trade.ChannelPostId, chat);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			await _context.SaveChangesAsync();
			await client.SendTextMessage("Start trading guys..", chat);
		}
	}
}