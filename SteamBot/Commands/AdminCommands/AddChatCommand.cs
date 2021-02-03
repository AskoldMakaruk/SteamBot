using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Model;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamBot.Commands.AdminCommands
{
	public class AddChatCommand : StaticCommand
	{
		private readonly TelegramContext _context;
		private readonly SteamService _steamService;

		public AddChatCommand(TelegramContext context, SteamService steamService)
		{
			_context = context;
			_steamService = steamService;
		}

		public override bool SuitableFirst(Update message)
			=> message?.Message?.Text == "/addchat";


		public override async Task<Response> Execute(IClient client)
		{
			var update = await client.GetTextMessage();
			var chat = update.Chat;

			var account = _context.GetAccount(update);
			if (!account.IsAdmin)
			{
				return default;
			}

			if (chat.Type != ChatType.Group)
			{
				await client.SendTextMessage("Wrong chat type.", chat);
				return default;
			}


			var room = _context.ChatRooms.FirstOrDefault(ch => ch.ChatId == chat.Id);

			var configuredCorrectly = true;
			var permissions = new Dictionary<string, bool>
			{
				["CanSendMessages"] = true,
				["CanSendMediaMessages"] = true,
				["CanSendOtherMessages"] = true,
				["CanAddWebPagePreviews"] = true,
				["CanInviteUsers"] = false,
				["CanPinMessages"] = false,
				["CanChangeInfo"] = false,
				["CanSendPolls"] = false
			};

			var builder = new StringBuilder();
			var props = typeof(ChatPermissions).GetProperties();
			chat = await client.GetChat(chat);
			foreach (var prop in props)
			{
				if (permissions[prop.Name] != (bool?) prop.GetValue(chat.Permissions))
				{
					builder.AppendLine($"Incorrect permission: {prop.Name}. Expected {permissions[prop.Name]}");
					configuredCorrectly = false;
				}
			}

			if (!configuredCorrectly)
			{
				await client.SendTextMessage(builder.ToString(), chat);
				return default;
			}

			string text;
			if (room == null)
			{
				room = new ChatRoom
				{
					ChatId = chat.Id,
					InviteLink = await client.ExportChatInviteLink(chat)
				};
				await _context.ChatRooms.AddAsync(room);
				await _context.SaveChangesAsync();
				text = "Chat added.";
			}
			else
			{
				text = "Chat is already in db.";
			}


			//todo set pic
			await client.SetChatTitle($"Trading room #{room.Id}", chat);
			await client.SetChatDescription(chat, "Some descrition was set by bot!");

			var chats = _context.ChatRooms.ToList();
			await client.SendTextMessage(text + $"\n\nTotal chat count: {chats.Count}\nFree: {chats.Count(a => a.TradeId == null)}", chat);
			return default;
		}
	}
}