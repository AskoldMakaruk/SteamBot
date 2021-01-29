using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using Telegram.Bot.Types;

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
			if (!t.IsPrimitive && t != typeof(Decimal) && t != typeof(String))
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
	}
}