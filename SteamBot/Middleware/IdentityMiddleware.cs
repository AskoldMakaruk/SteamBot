using System.Threading.Tasks;
using BotFramework;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using SteamBot.Model;
using SteamBot.Services;
using Telegram.Bot.Types;

namespace SteamBot.Middleware
{
	public class AccountContext
	{
		public Account Account { get; set; }
	}

	public class IdentityMiddleware
	{
		private readonly UpdateDelegate _next;

		public IdentityMiddleware(UpdateDelegate next)
		{
			_next = next;
		}

		public Task Invoke(Update update, AccountContext accountContext, Database context)
		{
			if (update.GetUser() is not { } user)
			{
				return _next.Invoke(update);
			}

			accountContext.Account = context.GetAccount(user);

			return _next.Invoke(update);
		}
	}

	public static class UseIdentityMiddleware
	{
		public static void UseIdentity(this IAppBuilder builder)
		{
			builder.Services.AddScoped<AccountContext>();
			builder.UseMiddleware<IdentityMiddleware>();
		}
	}
}