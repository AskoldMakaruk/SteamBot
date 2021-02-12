using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SteamBot.Model;

namespace SteamBot.Services
{
	public class TranslationContainer
	{
		//public TranlsationLanguage RuTranslations;
		public TranlsationLanguage EngTranslations;

		public TranslationContainer()
		{
			//RuTranslations = new TranlsationLanguage("RU");
			EngTranslations = new TranlsationLanguage("EN");
		}

		public string this[string input] => EngTranslations[input];
		//{
		//	get
		//	{

		//		return input.ToUpper() switch
		//		{
		//			"EN" => EngTranslations[input],
		//			//"RU" => RuTranslations,
		//			_ => null
		//		};
		//	}
		//}
	}

	public class TranlsationLanguage
	{
		public string Locale;
		public ConcurrentDictionary<string, string> Translations = new();

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

	public class TranslationsService
	{
		private static readonly string[] Keys =
		{
			"AbsolutelySure",
			"BuyBtn",
			"CancelTrade",
			"CancelTradeConfirmationText",
			"EnterTradeUrlText",
			"JoinChatRoomText",
			"ListTradeItem",
			"MenuText",
			"MyFundsBtn",
			"MyStatsBtn",
			"MyTradesBtn",
			"NewTrade_ChooseSkin",
			"NewTrade_NothingFound",
			"NewTrade_SendItemText",
			"NewTradeBtn",
			"NoCurrentTradesText",
			"NoFreeChatRoomsError",
			"NoItemWithSuchFloatError",
			"PriceSetText",
			"SellBtn",
			"SellerTryingToBuyHisItemError",
			"SellSkin_EnterPrice",
			"SellSkin_Error",
			"SellSkin_SelectFloatErorr",
			"SellSkin_TradeCreated",
			"SendMoneyText",
			"SetPrice",
			"StartText",
			"TradeInProgressError",
			"TradeStartText",
			"WaitingForPriceText",
		};

		public static TranslationContainer Locales;
		private readonly Database _context;

		public string this[string input]
		{
			get
			{
				lock (Locales)
				{
					return Locales.EngTranslations[input];
				}
			}
		}

		public TranslationsService(Database context)
		{
			_context = context;
			//initializes keys
			if (Locales == null)
			{
				var translations = _context.Translations.ToList();
				foreach (var key in Keys)
				{
					var a = translations.FirstOrDefault(k => k.KeyName == key);
					if (a == null)
					{
						_context.Add(new Translation
						{
							En = key,
							KeyName = key,
							Ru = key
						});
					}
				}

				_context.SaveChanges();
			}

			Locales ??= new TranslationContainer();
		}

		private const char s = ';';

		public MemoryStream ExportCsv()
		{
			var translations = _context.Translations.ToList();

			var memoryStream = new MemoryStream();
			using var writer = new StreamWriter(memoryStream, leaveOpen: true);

			writer.WriteLine($"Key{s}En{s}Ru");

			foreach (var translation in translations)
			{
				writer.WriteLine($"{translation.KeyName}{s}{translation.En}{s}{translation.Ru}");
			}

			return memoryStream;
		}

		public int ImportCsv(MemoryStream stream)
		{
			var text = Encoding.UTF8.GetString(stream.ToArray());
			var lines = text.Split('\n');
			var list = _context.Translations.ToList();

			foreach (var line in lines)
			{
				var words = line.Split(s);
				var translation = list.FirstOrDefault(a => a.KeyName == words[0]);
				if (translation == null)
				{
					translation = new Translation
					{
						KeyName = words[0]
					};
					_context.Add(translation);
				}

				translation.En = words[1];
				translation.Ru = words[2];
			}

			return _context.SaveChanges();
		}

		public void ReloadTranslations()
		{
			var translations = _context.Translations.AsNoTracking().ToList();

			lock (Locales)
			{
				//Locales.RuTranslations.Translations = new();
				Locales.EngTranslations.Translations = new();

				foreach (var translation in translations)
				{
					//Locales.RuTranslations.Translations.TryAdd(translation.KeyName, translation.Ru);
					Locales.EngTranslations.Translations.TryAdd(translation.KeyName, translation.En);
				}
			}
		}
	}
}