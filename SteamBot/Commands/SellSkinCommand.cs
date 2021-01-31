﻿using System;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using Microsoft.Extensions.Configuration;
using SteamBot.Database;
using SteamBot.Localization;
using SteamBot.Model;
using SteamBot.Services;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	public class SellSkinCommand : StaticCommand
	{
		private readonly SteamService _steamService;
		private readonly TelegramContext _context;
		private readonly long ChannelId;

		public SellSkinCommand(SteamService steamService, TelegramContext context, IConfiguration configuration)
		{
			_steamService = steamService;
			_context = context;
			ChannelId = long.Parse(configuration["ChannelId"]);
		}

		public override bool SuitableFirst(Update message) => message?.CallbackQuery?.Data.Contains("Sell") ?? false;

		public override async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var account = _context.GetAccount(query.From);
			var skin = await _context.Skins.FindAsync(int.Parse(query.Data.Split(' ')[0]));

			account.CurrentTrade = new TradeItem
			{
				Skin = skin,
			};

			await client.SendTextMessage(Texts.ResourceManager.GetString("EnterPrice"));

			account.CurrentTrade.Price = await client.GetValue<double>();


			var fl = query.Message.GetFloat() ?? default;

			if (skin.GetImage(fl) == null)
			{
				await _steamService.GetSteamItem(skin, fl);
				skin = await _context.Skins.FindAsync(skin.Id);
			}


			await using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var trade = new Trade
					{
						Seller = account,
						Status = TradeStatus.Open,
						TradeItem = account.CurrentTrade,
					};

					await _context.Trades.AddAsync(trade);
					await _context.SaveChangesAsync();

					var me = await client.GetMe();
					var message = await client.SendSkin(skin, skin.ToMessage(account.CurrentTrade.Price, fl), replyMarkup: Keys.ChannelMarkup(trade.Id, me.Username), chatid: new Chat {Id = ChannelId}, fl: fl);

					trade.ChannelPostId = message.MessageId;
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					await transaction.RollbackAsync();
					await client.SendTextMessage("Error proccessing your request.");
				}
			}

			await client.SendTextMessage(Texts.ResourceManager.GetString("TradeCreated"));
			return new Response();
		}
	}
}