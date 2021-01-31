using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using SteamApi.Model;
using SteamBot.Localization;

namespace SteamBot
{
	public static class Helper
	{
		public const string Star = "★";
		public const string StatTrak = "StatTrak™";

		public static ResourceManager ResourceManager => Texts.ResourceManager;

		public static string GetFloatName(float value, string culture = "en-EN")
		{
			var c = CultureInfo.GetCultureInfo(culture);
			if (value >= 0 && value <= 0.07)
			{
				return ResourceManager.GetString("Float_Factory_New", c);
				return "Factory New";
			}

			if (value <= 0.15)
			{
				return ResourceManager.GetString("Float_Minimal_Wear", c);
				return "Minimal Wear";
			}

			if (value <= 0.38)
			{
				return ResourceManager.GetString("Float_Field_Tested", c);
				return "Field-Tested";
			}

			if (value <= 0.45)
			{
				return ResourceManager.GetString("Float_Well_Worn", c);
				return "Well-Worn";
			}

			return ResourceManager.GetString("Float_Battle_Scarred", c);
			return "Battle-Scarred";
		}

		public static string[] Floats(string culture = "en-EN")
		{
			var c = CultureInfo.GetCultureInfo(culture);
			return new[]
			{
				ResourceManager.GetString("Float_Factory_New", c),
				ResourceManager.GetString("Float_Minimal_Wear", c),
				ResourceManager.GetString("Float_Field_Tested", c),
				ResourceManager.GetString("Float_Well_Worn", c),
				ResourceManager.GetString("Float_Battle_Scarred", c)
			};
		}

		public static bool IsFloated(string hashName)
		{
			return Floats().Any(hashName.Contains);
		}

		public static bool TryGetFloatValue(string floatName, out float value, string culture = "en-EN")
		{
			floatName = floatName.Trim();
			value = -1f;
			var c = CultureInfo.GetCultureInfo(culture);
			if (floatName == ResourceManager.GetString("Float_Factory_New", c)?.Trim())
			{
				value = 0.06f;
			}

			if (floatName == ResourceManager.GetString("Float_Minimal_Wear", c)?.Trim())
			{
				value = 0.14f;
			}

			if (floatName == ResourceManager.GetString("Float_Field_Tested", c)?.Trim())
			{
				value = 0.37f;
			}

			if (floatName == ResourceManager.GetString("Float_Well_Worn", c)?.Trim())
			{
				value = 0.44f;
			}

			if (floatName == ResourceManager.GetString("Float_Battle_Scarred", c)?.Trim())
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

		public static (string WeaponName, string SkinName) GetNormalizedName(string hashName)
		{
			var IsFloated = Helper.IsFloated(hashName);
			var HasDelimiter = hashName.Contains('|');
			var marketHashName = hashName;

			var IsKnife = marketHashName.Contains(Helper.Star);
			if (IsKnife)
			{
				marketHashName = marketHashName.Replace(Helper.Star, String.Empty).Trim();
			}

			var IsStatTrak = marketHashName.Contains(Helper.StatTrak);
			if (IsStatTrak)
			{
				marketHashName = marketHashName.Replace(Helper.StatTrak, String.Empty).Trim();
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
				skinName = WeaponName;
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