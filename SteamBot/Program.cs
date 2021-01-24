using System;
using System.Globalization;
using System.IO;
using System.Linq;
using BotFramework.Handlers;
using Newtonsoft.Json.Linq;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
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
			var json = File.ReadAllText("compact.json");
			var jobject = JObject.Parse(json);
			var list = jobject.Children().OfType<JProperty>().Select(a => new {Name = a.Name, Price = a.Value.Value<double>()}).ToList();

			Console.WriteLine("Star items: " + list.Count(a => a.Name.Contains("★")));
			Console.WriteLine("AK's " + list.Count(a => a.Name.Contains("AK-47")));
			Console.WriteLine("IsStatTrak items: " + list.Count(a => a.Name.Contains("IsStatTrak")));

			//Console.ReadLine();
			//foreach (var child in jobject.Children().OfType<JProperty>())
			//{
			//	Console.WriteLine($"{child.Name}: {child.Value.Value<double>()}");
			//}


			new HandlerConfigurationBuilder("823973981:AAGYpq1Eyl_AAYGXLeW8s28uCH89S7fsHZA", typeof(Program).Assembly)
				.UseConsoleDefaultLogger()
				.WithCustomNinjectModules(new Injector())
				.Build()
				.RunInMemoryHandler();
		}
	}

	public class Injector : NinjectModule
	{
		public override void Load()
		{
			Bind<TelegramContext>().To<TelegramContext>();
			Bind<SteamService>().To<SteamService>();
			Bind<SteamApiClient>().ToMethod(_ => new SteamApiClient("GBY60z2F65MOmMmqUUEnecpfb4A"));
		}
	}
}