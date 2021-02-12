using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Localization;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static SteamBot.Services.TranslationsService;

namespace SteamBot.Commands
{
	public class NewTradeCommand : StaticCommand
	{
		private readonly SteamService _steamService;

		public NewTradeCommand(SteamService steamService)
		{
			_steamService = steamService;
		}

		public override bool SuitableLast(Update message) => message?.Message?.Text == Locales["NewTradeBtn"];

		public override async Task Execute(IClient client)
		{
			var _ = await client.GetUpdate();
			await client.SendTextMessage(Locales["NewTrade_SendItemText"]);
			var message = await client.GetTextMessage();
			while (true)
			{
				var skins = _steamService.FindItems(message.Text).ToList();

				var sk = skins.FirstOrDefault(a => a.SearchName == message.Text);
				if (skins.Count == 1 || sk is not null)
				{
					var skin = sk ?? skins.FirstOrDefault();
					var fl = skin?.GetPrice().Float;

					if (skin?.GetImage(fl)?.Bytes == null)
					{
						await _steamService.GetSteamItem(skin, fl);
						//skin = await _context.Skins.FindAsync(skin.Id);
					}

					await client.SendSkin(skin);
				}
				else if (skins.Count > 1)
				{
					ReplyKeyboardMarkup markup = skins.Select(a => a.SearchName).GroupElements(2).Select(a => a.ToArray()).ToArray();

					markup.ResizeKeyboard = true;
					markup.OneTimeKeyboard = true;
					await client.SendTextMessage(Locales["NewTrade_ChooseSkin"], replyMarkup: markup);
				}
				else
				{
					await client.SendTextMessage(Locales["NewTrade_NothingFound"]);
				}

				message = await client.GetTextMessage();
			}
		}
	}

	//public class ChannelCommand : StaticCommand
	//{
	//	public override bool SuitableFirst(Update message) => message?.ChannelPost != null;

	//	public override async Task Execute(IClient client)
	//	{
	//		var update = await client.GetUpdate();
	//		Console.WriteLine(update.ChannelPost.Chat.Id);
	//		await client.SendTextMessage("Message from bot", update.ChannelPost.Chat.Id);
	//		return default;
	//	}
	//}
}