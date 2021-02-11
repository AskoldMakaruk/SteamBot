using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BotFramework;
using BotFramework.Abstractions;
using BotFramework.Clients;
using BotFramework.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using SteamApi;
using SteamBot.Middleware;
using SteamBot.Services;
using Telegram.Bot;

namespace SteamBot
{
	public static class Program
	{
		public static UpdateDelegate app;

		private static void Main()
		{
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");

			var host = Host.CreateDefaultBuilder()
				.ConfigureHostConfiguration(configurationBuilder => configurationBuilder.AddEnvironmentVariables())
				.ConfigureServices(services =>
				{
					var builder = new AppBuilder(services);
					services.ConfigureServices(builder);

					builder.ConfigureMiddlewares();

					(_, app) = builder.Build();
				})
				.Build();


			var translations = host.Services.GetService<TranslationsService>();
			translations?.ReloadTranslations();

			var bot = host.Services.GetService<ITelegramBotClient>()!;
			bot!.OnUpdate += (_, eventArgs) => app(eventArgs.Update);

			bot.StartReceiving();
			Console.ReadLine();
		}

		public static void ConfigureServices(this IServiceCollection services, IAppBuilder builder)
		{
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

		public static void ConfigureMiddlewares(this IAppBuilder builder)
		{
			builder.UseIdentity();
			builder.UseStaticCommands();
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