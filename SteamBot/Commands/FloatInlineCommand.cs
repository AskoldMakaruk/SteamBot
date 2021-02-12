using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Services;
using Telegram.Bot.Types;
using static SteamBot.Services.TranslationsService;

namespace SteamBot.Commands
{
	public class FloatInlineCommand : StaticCommand
	{
		private readonly Database _context;

		public FloatInlineCommand(Database context)
		{
			_context = context;
		}

		public override bool SuitableFirst(Update message) => Helper.FloatsNames().Any(a => message?.CallbackQuery?.Data?.Contains(a) ?? false);

		public override async Task Execute(IClient client)
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
					await client.AnswerCallbackQuery(Locales["NoItemWithSuchFloatError"]);
				}

				if (Equals(seletedFloat, fl))
				{
					seletedFloat = null;
				}
				else
				{
					seletedFloat = fl;
				}

				await client.UpdateSkin(query, skin, seletedFloat, query.Message.IsStatTrak());


				//todo check if message 
				//todo edit image?
			}
		}
	}
}