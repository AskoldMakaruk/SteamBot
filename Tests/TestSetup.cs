using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Abstractions;
using BotFramework.Clients;
using BotFramework.HostServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SteamBot;
using SteamBot.Services;

namespace Tests
{
	public class TestSetup
	{
		public UpdateDelegate App;
		public DebugClient Client;
		public IHost AppHost;

		public TestSetup()
		{
			Client = new DebugClient();

			AppHost = Host.CreateDefaultBuilder()
				.ConfigureHostConfiguration(configurationBuilder => configurationBuilder.AddEnvironmentVariables())
				.ConfigureLogging((context, logging) =>
				{
					logging.AddConsole();
					logging.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Warning);
				})
				.ConfigureAppDebug(appBuilder =>
				{
					var services = appBuilder.Services;

					services.ConfigureServices(appBuilder);

					services.RemoveAll(typeof(IUpdateConsumer));
					services.RemoveAll<Database>();
					services.RemoveAll<DbContextOptions<Database>>();

					services.AddTransient<IUpdateConsumer, DebugClient>(_ => Client);

					services.AddDbContext<Database>(options =>
					{
						var configuration = Program.GetConfiguration();
						options.UseNpgsql(configuration.GetConnectionString("TestConnection"));
						options.UseLazyLoadingProxies();
					});

					(_, App) = appBuilder.Build();
				})
				.Build();


			var database = AppHost.Services.GetService<Database>();
			if (database == null)
			{
				throw new Exception("Database is not initialized.");
			}

			var connection = database.Database.GetConnectionString();

			if (!connection.Contains("IntegrationTests_SteamBot"))
			{
				throw new Exception("Invalid database.");
			}

			ClearDb(database);
		}

		private void ClearDb(Database database)
		{
			database.Accounts.RemoveRange(database.Accounts);
			database.Images.RemoveRange(database.Images);
			database.Skins.RemoveRange(database.Skins);
			database.Trades.RemoveRange(database.Trades);
			database.SteamItems.RemoveRange(database.SteamItems);
			database.ChatRooms.RemoveRange(database.ChatRooms);
			//database.Translations.RemoveRange(database.Translations);

			database?.SaveChanges();
		}

		public async Task FillDb(Database database)
		{
			var translations = AppHost.Services.GetService<TranslationsService>();
			translations?.ReloadTranslations();

			var steamService = AppHost.Services.GetService<SteamService>();
			await steamService.UpdateDb();
		}
	}
}