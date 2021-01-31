﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SteamBot.Model
{
	public partial class Skin
	{
		public string GetHashName(float? fl) => GetHashName(fl == null ? GetFloats("en-EN").First() : Helper.GetFloatName((float) fl));
		public string GetHashName(string fl) => $"{GetHashName()} {(IsFloated ? $"({fl})" : String.Empty)}";
		private string GetHashName() => $"{(IsKnife ? Helper.Star : String.Empty)} {(IsStatTrak ? Helper.StatTrak : String.Empty)} {WeaponName} | {SkinName}";

		public List<double> GetPrices() => Prices.Select(a => a.Value).ToList();

		public Image GetImage(float? fl = null)
		{
			return GetPrice(fl)?.Image;
		}

		public SkinPrice GetPrice(float? fl = null)
		{
			if (fl == null)
			{
				return Prices.OrderBy(a => a.Float).First();
			}

			return Prices.FirstOrDefault(a => a.FloatName == Helper.GetFloatName((float) fl));
		}

		public void ParseHashName(string hashName)
		{
			IsFloated = Helper.IsFloated(hashName);
			IsKnife = hashName.Contains(Helper.Star);
			(WeaponName, SkinName) = Helper.GetNormalizedName(hashName);
			SearchName = $"{WeaponName} {SkinName}";
		}

		public string ToMessage(double? price = null, float? fl = null)
		{
			string priceTxt = null;
			if (price != null)
			{
				priceTxt = $"Seller's price: {price.Value.ToString("F", CultureInfo.InvariantCulture)}$";
			}

			string priceTexts;
			if (fl == 0)
			{
				var prices = GetPrices();
				priceTexts = $"{prices.Min().ToString("F", CultureInfo.InvariantCulture)}$ - {prices.Max().ToString("F", CultureInfo.InvariantCulture)}$";
			}
			else
			{
				priceTexts = $"{GetPrice(fl)?.Value.ToString("F", CultureInfo.InvariantCulture)}$";
			}


			return $"*{SearchName}*\n{Helper.GetFloatName(fl ?? default)}\nSteam Market Price: {priceTexts}\n\n{priceTxt}";
		}

		public static Skin FromMessage()
		{
			return null;
		}

		public List<string> GetFloats(string culture)
		{
			return Prices.OrderBy(a => a.Float).Select(a => Helper.GetFloatName(a.Float ?? default, culture)).Distinct().ToList();
		}
	}
}