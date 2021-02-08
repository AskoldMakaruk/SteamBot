using System;
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
		private readonly TelegramContext _context;
		private readonly SteamService _steamService;
		private readonly long ChannelId;

		public SellSkinCommand(SteamService steamService, TelegramContext context, IConfiguration configuration)
		{
			_steamService = steamService;
			_context = context;
			ChannelId = Int64.Parse(configuration["ChannelId"]);
		}

		public override bool SuitableFirst(Update message) => message?.CallbackQuery?.Data.Contains("Sell") ?? false;

		public override async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();

			var fl = query.Message.GetFloat() ;
			var skin = await _context.Skins.FindAsync(Int32.Parse(query.Data.Split(' ')[0]));

			if (fl == null && skin.IsFloated)
			{
				await client.AnswerCallbackQuery(query.Id, "Select float please.");
				return new();
			}

			var account = _context.GetAccount(query.From);
		

			account.CurrentTrade = new Trade
			{
				Skin = skin,
				Seller = account,
				Status = TradeStatus.Open,
			};

			await client.SendTextMessage(Texts.ResourceManager.GetString("EnterPrice"));

			account.CurrentTrade.StartPrice = await client.GetValue<double>();


			

			if (skin.GetImage(fl) == null)
			{
				await _steamService.GetSteamItem(skin, fl);
				skin = await _context.Skins.FindAsync(skin.Id);
			}


			await using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var trade = account.CurrentTrade;
					await _context.Trades.AddAsync(trade);
					await _context.SaveChangesAsync();

					var me = await client.GetMe();
					var message = await client.SendSkin(skin, skin.ToMessage(price: account.CurrentTrade.StartPrice, fl: fl), replyMarkup: Keys.ChannelMarkup(trade.Id, me.Username), chatid: new Chat {Id = ChannelId}, fl: fl);

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

			await client.SendTextMessage(Texts.ResourceManager.GetString("TradeCreated"),replyMarkup:Keys.StartKeys());
			return new Response();
		}
	}
}