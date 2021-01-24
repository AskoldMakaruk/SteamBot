using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SteamBot.Localization;

namespace SteamBot
{
	public static class Helper
	{
		public const string Star = "★";
		public const string StatTrak = "StatTrak™";

		public static string GetFloatName(float value, string culture = "en-EN")
		{
			Texts.Culture = CultureInfo.GetCultureInfo(culture);

			if (value >= 0 && value <= 0.07)
			{
				return Texts.Float_Factory_New;
				return "Factory New";
			}

			if (value <= 0.15)
			{
				return Texts.Float_Minimal_Wear;
				return "Minimal Wear";
			}

			if (value <= 0.38)
			{
				return Texts.Float_Field_Tested;
				return "Field-Tested";
			}

			if (value <= 0.45)
			{
				return Texts.Float_Well_Worn;
				return "Well-Worn";
			}

			return Texts.Float_Battle_Scarred;
			return "Battle-Scarred";
		}

		public static bool TryGetFloatValue(string floatName, out float value, string culture = "en-EN")
		{
			floatName = floatName.Trim();
			value = -1f;
			Texts.Culture = CultureInfo.GetCultureInfo(culture);
			if (floatName == Texts.Float_Factory_New.Trim())
			{
				value = 0.6f;
			}

			if (floatName == Texts.Float_Minimal_Wear.Trim())
			{
				value = 0.14f;
			}

			if (floatName == Texts.Float_Field_Tested.Trim())
			{
				value = 0.37f;
			}

			if (floatName == Texts.Float_Well_Worn.Trim())
			{
				value = 0.44f;
			}

			if (floatName == Texts.Float_Battle_Scarred.Trim())
			{
				value = 0.99f;
			}

			return value > 0;
		}


		public static IEnumerable<TSource> DistinctBy<TSource, TKey>
			(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var seenKeys = new HashSet<TKey>();
			foreach (var element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}

		public static IEnumerable<IEnumerable<T>> GroupElements<T>(this IEnumerable<T> fullList, int batchSize)
		{
			using (var enumerator = fullList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					yield return Take(enumerator);
				}
			}

			IEnumerable<T> Take(IEnumerator<T> enumerator)
			{
				for (var i = 0; i < batchSize; i++)
				{
					yield return enumerator.Current;
					if (i + 1 >= batchSize)
					{
						continue;
					}

					if (!enumerator.MoveNext())
					{
						break;
					}
				}
			}
		}

		public static IFormatProvider Provider
		{
			get
			{
				var result = new NumberFormatInfo();
				return result;
			}
		}
	}
}