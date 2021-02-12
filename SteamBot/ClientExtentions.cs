using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
using SteamBot.Commands;
using SteamBot.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using static SteamBot.Services.TranslationsService;


namespace SteamBot
{
	public static class ClientExtentions
	{
		public static async Task<CallbackQuery> GetCallbackQuery(this IClient client)
		{
			return (await client.GetUpdate(a => a.CallbackQuery != null)).CallbackQuery;
		}

		public static async Task<T> GetValue<T>(this IClient client)
		{
			var t = typeof(T);
			if (!t.IsPrimitive && t != typeof(decimal) && t != typeof(string))
			{
				throw new NotSupportedException($"Type {t} is not supported");
			}


			while (true)
			{
				var message = await client.GetTextMessage();

				if (TryParse(message.Text, out var result))
				{
					return result;
				}
			}

			bool TryParse(string text, out T value)
			{
				var type = typeof(T);
				Type[] argTypes = {typeof(string), typeof(NumberStyles), typeof(IFormatProvider), type.MakeByRefType()};
				var methodInfo = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, argTypes, null);

				if (methodInfo != null)
				{
					object[] parameters = {text, NumberStyles.Any, CultureInfo.InvariantCulture, null};

					var success = (bool) methodInfo.Invoke(null, parameters);
					if (success)
					{
						value = (T) parameters.Last();
						return true;
					}
				}

				value = default;
				return false;
			}
		}


		public static async Task<Message> GetTradeCancelMessage(this IClient client)
		{
			return (await client.GetUpdate(update => update?.Message?.Text == Locales["AbsolutelySure"])).Message;
		}

		public static async Task<Message> GetTextMessage(this IClient client, Func<Message, bool> predicate)
		{
			return (await client.GetUpdate(update => update?.Message != null && predicate.Invoke(update.Message))).Message;
		}

		public static async Task<Message> SendSkin(this IClient client, Skin skin, string text = null, float? fl = default, IReplyMarkup replyMarkup = null, Chat? chatid = null)
		{
			var image = skin.Prices.OrderBy(a => a.Float).FirstOrDefault(a => a.Image != null)?.Image;
			replyMarkup ??= Keys.FloatMarkup(skin, "en-EN", fl, skin.Prices.Any(a => a.StatTrak == true) ? false : null);
			text ??= skin.ToMessage();

			ChatId chat = null;
			if (chatid != null)
			{
				chat = chatid;
			}

			if (image == null)
			{
				return await client.SendTextMessage(text, parseMode: ParseMode.Markdown, replyMarkup: replyMarkup, chatId: chat);
			}

			await using MemoryStream stream = new(image.Bytes);
			return await client.SendPhoto(new InputOnlineFile(stream, "skin.png"), caption: text, parseMode: ParseMode.Markdown, replyMarkup: replyMarkup, chatId: chat);
		}

		public static async Task UpdateSkin(this IClient client, CallbackQuery query, Skin skin, float? seletedFloat, bool? isStatTrak)
		{
			var text = skin.ToMessage(fl: seletedFloat, statTrak: isStatTrak);

			if (query.InlineMessageId != null)
			{
				await client.EditMessageCaption(query.InlineMessageId, text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "en-EN", seletedFloat, isStatTrak));
			}
			else
			{
				await client.EditMessageCaption(query.Message.MessageId, text, parseMode: ParseMode.Markdown, replyMarkup: Keys.FloatMarkup(skin, "en-EN", seletedFloat, isStatTrak));
			}
		}
	}
}