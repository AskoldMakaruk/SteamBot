using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SteamBot.Model;

namespace SteamBot.Services
{
	public class TranslationContainer
	{
		public TranlsationLanguage RuTranslations;
		public TranlsationLanguage EngTranslations;
		private readonly Database _context;

		public TranslationContainer(Database context)
		{
			_context = context;

			RuTranslations = new TranlsationLanguage("RU");
			EngTranslations = new TranlsationLanguage("EN");
		}

		public TranlsationLanguage this[string input]
		{
			get
			{
				return input.ToUpper() switch
				{
					"EN" => EngTranslations,
					"RU" => RuTranslations,
					_ => null
				};
			}
		}

		public void ReloadTranslations()
		{
			var translations = _context.Translations.AsNoTracking().ToList();

			RuTranslations.Translations = new Dictionary<string, string>();
			EngTranslations.Translations = new Dictionary<string, string>();

			foreach (var translation in translations)
			{
				RuTranslations.Translations.Add(translation.KeyName, translation.Ru);
				EngTranslations.Translations.Add(translation.KeyName, translation.En);
			}
		}
	}

	public class TranlsationLanguage
	{
		public string Locale;
		public Dictionary<string, string> Translations = new();

		public TranlsationLanguage(string locale)
		{
			Locale = locale;
		}

		public string this[string input]
		{
			get
			{
				if (Translations.ContainsKey(input))
				{
					return Translations[input];
				}

				this[input] = input;

				return input;
			}
			set => Translations[input] = value;
		}
	}

	//make disposable
	public class TranslationsService
	{
		public static TranslationContainer Locales;
		private readonly Database _context;
		private readonly IServiceProvider _provider;
		public TranlsationLanguage this[string input] => Locales[input];

		public TranslationsService(Database context, IServiceProvider provider)
		{
			_context = context;
			_provider = provider;
			Locales ??= new TranslationContainer(context);
		}

		public MemoryStream ExportCsv()
		{
			var translations = _context.Translations.ToList();

			var memoryStream = new MemoryStream();
			using var writer = new StreamWriter(memoryStream, leaveOpen: true);

			writer.WriteLine("Key,En,Ru");

			foreach (var translation in translations)
			{
				writer.WriteLine($"{translation.KeyName},{translation.En},{translation.Ru}");
			}

			return memoryStream;
		}

		public void ImportCsv(string csv) { }

		public void ReloadTranslations()
		{
			Locales.ReloadTranslations();
		}

		public void SaveChanges()
		{
			var eng = Locales.EngTranslations;
			var rus = Locales.RuTranslations;

			var translations = eng.Translations.FullOuterJoin(rus.Translations, pair => pair.Key, pair => pair.Key, (en, ru, arg3) => new Translation
			{
				En = en.Value,
				Ru = ru.Value,
				KeyName = ru.Key ?? en.Key
			}).ToList();

			using (var scope = _provider.CreateScope())
			{
				var context = scope.ServiceProvider.GetService<Database>();
				var dbtranslations = context?.Translations.ToList();
				foreach (var translation in translations)
				{
					if (dbtranslations?.FirstOrDefault(a => a.KeyName == translation.KeyName) is null)
					{
						context?.Translations.Add(translation);
					}
				}

				context?.SaveChanges();
			}
		}
	}
}