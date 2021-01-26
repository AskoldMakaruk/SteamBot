using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using BotFramework.Handlers;
using Microsoft.Extensions.Configuration;
using Ninject;
using Ninject.Modules;
using SteamApi;
using SteamBot.Database;
using SteamBot.Services;

namespace SteamBot
{
	class Program
	{
		static void Main()
		{
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
			//var json = File.ReadAllText("compact.json");
			//var jobject = JObject.Parse(json);
			//var list = jobject.Children().OfType<JProperty>().Select(a => new {Name = a.Name, Price = a.Value.Value<double>()}).ToList();

			//Console.WriteLine("Star items: " + list.Count(a => a.Name.Contains("★")));
			//Console.WriteLine("AK's " + list.Count(a => a.Name.Contains("AK-47")));
			//Console.WriteLine("IsStatTrak items: " + list.Count(a => a.Name.Contains("IsStatTrak")));

			//Console.ReadLine();
			//foreach (var child in jobject.Children().OfType<JProperty>())
			//{
			//	Console.WriteLine($"{child.Name}: {child.Value.Value<double>()}");
			//}

			var st = Stopwatch.StartNew();
			var kernel = new StandardKernel(new Injector());
			var steamService = kernel.Get<SteamService>();
			steamService.UpdateDb().Wait();
			Console.WriteLine(st.Elapsed);

			var config = GetConfiguration();

			new HandlerConfigurationBuilder(config["BotToken"], typeof(Program).Assembly)
				.UseConsoleDefaultLogger()
				.WithCustomNinjectModules(new Injector())
				.Build()
				.RunInMemoryHandler();
		}

		public static IConfiguration GetConfiguration()
		{
#if DEBUG
			const string env = "Development";
#endif
#if RELEASE
			const string env = "Production";
#endif
			string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
			return new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile($"{assemblyFolder}/appsettings.json", false, true)
				.AddJsonFile($"{assemblyFolder}/appsettings.{env}.json", false, true)
				.Build();
		}
	}

	public class Injector : NinjectModule
	{
		public override void Load()
		{
			Bind<TelegramContext>().To<TelegramContext>();
			Bind<SteamService>().To<SteamService>();
			Bind<SteamApiClient>().ToMethod(_ => new SteamApiClient(Program.GetConfiguration()["SteamApiToken"]));
			Bind<IConfiguration>().ToMethod(_ => Program.GetConfiguration());
		}
	}
}