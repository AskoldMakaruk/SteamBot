using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using SteamBot.Services;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using static SteamBot.Services.TranslationsService;

namespace Tests.Commands
{
	public class CommandTests
	{
		private UpdateDelegate app = null;
		private DebugClient client;
		private TranslationsService translations;
		private IHost host;

		[SetUp]
		public void SetUp()
		{
			var setup = new TestSetup();
			app = setup.App;
			client = setup.Client;
			host = setup.AppHost;
		}

		[Test]
		public async Task StartCommandTest()
		{
			var scope = host.Services.CreateScope();

			translations = scope.ServiceProvider.GetService<TranslationsService>();

			await app(Message("/start"));

			Assert.AreEqual(translations["EN"]["EnterTradeUrlText"], (await client.GetRequest<SendMessageRequest>()).Text);

			await app(Message("some non url text"));

			Assert.AreEqual(translations["EN"]["EnterTradeUrlText"], (await client.GetRequest<SendMessageRequest>()).Text);

			await app(Message("https://com.com"));

			Assert.AreEqual(translations["EN"]["StartText"], (await client.GetRequest<SendMessageRequest>()).Text);

			translations.SaveChanges();
			scope.Dispose();
		}

		public static Update Message(string text) => new()
		{
			Message = new()
			{
				From = From,
				Text = text
			}
		};

		public static User From => new()
		{
			Id = 1,
			Username = "UserName",
		};
	}
}