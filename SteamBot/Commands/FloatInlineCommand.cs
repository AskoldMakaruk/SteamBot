using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Responses;
using SteamBot.Database;
using SteamBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamBot.Commands
{
	public class FloatInlineCommand : StaticCommand
	{
		private readonly TelegramContext _context;
		private readonly SteamService _steamService;

		public FloatInlineCommand(SteamService steamService, TelegramContext context)
		{
			_steamService = steamService;
			_context = context;
		}

		public override bool SuitableFirst(Update message) => Helper.FloatsNames().Any(a => message?.CallbackQuery?.Data?.Contains(a) ?? false);

		public override async Task<Response> Execute(IClient client)
		{
			var query = await client.GetCallbackQuery();
			var skin = await _context.GetSkinAsync(query);


			var flN = query.GetFloat();
			var seletedFloat = query.Message.GetFloat();

			if (flN is >= 0 and <= 1)
			{
				var fl = (float) flN;
				if (skin.GetPrice(fl) == null)
				{
					await client.AnswerCallbackQuery("Нет предмета с таким качеством.");
				}
				if (Equals(seletedFloat, fl))
				{
					seletedFloat = null;
				}
				else
				{
					seletedFloat = fl;
				}

				var text = skin.ToMessage(fl:seletedFloat);

				//todo check if message 
				//todo edit image?
			

				if (query.InlineMessageId != null)
				{
					await client.EditMessageCaption(query.InlineMessageId, text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU", seletedFloat));
				}
				else
				{
					await client.EditMessageCaption(query.Message.MessageId, text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "ru-RU", seletedFloat));
				}
			}

			return new Response();
		}
	}
}