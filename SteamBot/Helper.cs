using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using SteamApi.Model;
using SteamBot.Localization;
using Telegram.Bot.Types;

namespace SteamBot
{
	public static class Helper
	{
		public const string Star = "★";
		public const string StatTrak = "StatTrak™";

		public const string Float_Factory_New = "Factory New";
		public const string Float_Minimal_Wear = "Minimal Wear";
		public const string Float_Field_Tested = "Field-Tested";
		public const string Float_Well_Worn = "Well-Worn";
		public const string Float_Battle_Scarred = "Battle-Scarred";

		public static IFormatProvider Provider
		{
			get
			{
				var result = new NumberFormatInfo();
				return result;
			}
		}

		public static bool IsStatTrak(this Message message) =>
			(message?.Text ?? message?.Caption)?
			.Split('\n')[0]
			.Contains(StatTrak) ?? false;

		public static string GetFloatName(float value, string culture = "en-EN")
		{
			return Floats(culture).First(a => a.Value.Start <= value && value <= a.Value.End).Key;
		}

		public static bool TryGetFloatValue(string floatName, out float value, string culture = "en-EN")
		{
			var floats = Floats(culture);
			floatName = floatName.Trim();
			if (floats.ContainsKey(floatName))
			{
				value = floats[floatName].End - 0.01f;
				return true;
			}

			value = -1f;
			return false;
		}

		public static IReadOnlyDictionary<string, (float Start, float End)> Floats(string culture = "en-EN")
		{
			var c = CultureInfo.GetCultureInfo(culture);
			return new Dictionary<string, (float Start, float End)>
			{
				[Float_Factory_New] = new(0f, 0.07f),
				[Float_Minimal_Wear] = new(0.08f, 0.15f),
				[Float_Field_Tested] = new(0.16f, 0.38f),
				[Float_Well_Worn] = new(0.39f, 0.45f),
				[Float_Battle_Scarred] = new(0.46f, 1f)
			};
		}

		public static string[] FloatsNames(string culture = "en-EN")
		{
			return Floats(culture).Keys.ToArray();
		}

		public static bool IsFloated(string hashName)
		{
			return FloatsNames().Any(hashName.Contains);
		}

		public static float GetFloatValue(string floatName, string culture = "en-EN")
		{
			TryGetFloatValue(floatName, out var res, culture);
			return res;
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

		public static IEnumerable<TResult> FullOuterJoin<TA, TB, TKey, TResult>(
			this IEnumerable<TA> a,
			IEnumerable<TB> b,
			Func<TA, TKey> selectKeyA,
			Func<TB, TKey> selectKeyB,
			Func<TA, TB, TKey, TResult> projection,
			TA defaultA = default(TA),
			TB defaultB = default(TB),
			IEqualityComparer<TKey> cmp = null)
		{
			cmp ??= EqualityComparer<TKey>.Default;
			var alookup = a.ToLookup(selectKeyA, cmp);
			var blookup = b.ToLookup(selectKeyB, cmp);

			var keys = new HashSet<TKey>(alookup.Select(p => p.Key), cmp);
			keys.UnionWith(blookup.Select(p => p.Key));

			var join = from key in keys
				from xa in alookup[key].DefaultIfEmpty(defaultA)
				from xb in blookup[key].DefaultIfEmpty(defaultB)
				select projection(xa, xb, key);

			return join;
		}

		public static (string WeaponName, string SkinName) GetNormalizedName(string hashName)
		{
			var IsFloated = Helper.IsFloated(hashName);
			var HasDelimiter = hashName.Contains('|');
			var marketHashName = hashName;

			var IsKnife = marketHashName.Contains(Star);
			if (IsKnife)
			{
				marketHashName = marketHashName.Replace(Star, String.Empty).Trim();
			}

			var IsStatTrak = marketHashName.Contains(StatTrak);
			if (IsStatTrak)
			{
				marketHashName = marketHashName.Replace(StatTrak, String.Empty).Trim();
			}

			var delimiterIndx = marketHashName.IndexOf('|');

			var WeaponName = HasDelimiter ? marketHashName[..delimiterIndx].Trim() : marketHashName.Trim();

			string skinName;
			if (HasDelimiter)
			{
				skinName = (IsFloated ? marketHashName[(delimiterIndx + 2)..marketHashName.IndexOf('(')] : marketHashName[(delimiterIndx + 2)..]).Trim();
			}
			else
			{
				skinName = "";
			}

			return (WeaponName, skinName);
		}

		public static string GetWeaponName(this MarketItemCompact compact)
		{
			return GetNormalizedName(compact.Name).WeaponName;
		}

		public static string GetSkinName(this MarketItemCompact compact)
		{
			return GetNormalizedName(compact.Name).SkinName;
		}

		public static bool IsFloated(this MarketItemCompact compact)
		{
			return IsFloated(compact.Name);
		}

		public static float? GetFloat(string text)
		{
			if (text == null)
			{
				return null;
			}

			if (Floats().Any(a => text.Contains(a.Key)))
			{
				return Floats().First(a => text.Contains(a.Key)).Value.End - 0.01f;
			}

			//if (Helper.TryGetFloatValue(floatString, out var fl) || Single.TryParse(floatString, NumberStyles.Any, Helper.Provider, out fl))
			return null;
		}


		public static float? GetFloat(this Message message) =>
			GetFloat(message?.Text ?? message?.Caption);

		public static float? GetFloat(this CallbackQuery query)
			=> GetFloat(query.Data);
	}
}