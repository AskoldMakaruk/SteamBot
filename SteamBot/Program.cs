using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using BotFramework;
using BotFramework.Abstractions;
using BotFramework.Clients;
using BotFramework.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SteamApi;
using SteamBot.Middleware;
using SteamBot.Services;
using Telegram.Bot;
using BotFramework.HostServices;

namespace SteamBot
{
	public static class Program
	{
		private static void Main()
		{
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");

			var host = Host.CreateDefaultBuilder()
				.ConfigureLogging((context, logging) =>
				{
					var config = context.Configuration.GetSection("Logging");

					logging.AddConfiguration(config);
					logging.AddConsole();

					logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
				})
				.UseConfigurationWithEnvironment()
				.ConfigureApp((app, context) => app.ConfigureServices())
				.Build();

			var translations = host.Services.GetService<TranslationsService>();
			translations?.ReloadTranslations();

			host.RunAsync();

			Console.ReadLine();
		}

		public static void ConfigureServices(this IAppBuilder builder)
		{
			var services = builder.Services;
			builder.UseIdentity();
			builder.UseMiddleware<DictionaryCreatorMiddleware>();
			builder.UseStaticCommands();

			services.AddTransient<IUpdateConsumer, Client>();
			services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(GetConfiguration()["BotToken"]));

			services.AddScoped<DictionaryContext>();

			services.AddDbContext<Database>(options =>
			{
				var configuration = Program.GetConfiguration();
				options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
				options.UseLazyLoadingProxies();
			});

			services.AddScoped<TranslationsService>();
			services.AddScoped<SteamService>();
			services.AddSingleton(_ => new SteamApiClient(Program.GetConfiguration()["SteamApiToken"]));
			services.AddSingleton(_ => Program.GetConfiguration());
		}

		public static IConfiguration GetConfiguration()
		{
#if DEBUG
			const string env = "Development";
#endif
#if RELEASE
			const string env = "Production";
#endif
			var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
			return new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile($"{assemblyFolder}/appsettings.json", false, true)
				.AddJsonFile($"{assemblyFolder}/appsettings.{env}.json", false, true)
				.Build();
		}
	}
}