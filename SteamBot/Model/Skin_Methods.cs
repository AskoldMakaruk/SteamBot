﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

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

		public void SetImage(Image value, float? fl = null)
		{
			var price = GetPrice(fl);
			if (price != null)
			{
				price.Image = value;
			}
		}

		public SkinPrice GetPrice(float? fl = null, bool statTrak = false)
		{
			if (fl == null)
			{
				return Prices.OrderByDescending(a => a.Float).ThenBy(a => a.StatTrak == statTrak).First();
			}

			return Prices.OrderByDescending(a => a.StatTrak == statTrak).FirstOrDefault(a => !IsFloated || a.FloatName == Helper.GetFloatName((float) fl));
		}

		public void ParseHashName(string hashName)
		{
			IsFloated = Helper.IsFloated(hashName);
			IsKnife = hashName.Contains(Helper.Star);
			(WeaponName, SkinName) = Helper.GetNormalizedName(hashName);
			SearchName = $"{WeaponName} {SkinName}";
		}

		public bool StatTrakable => Prices.Any(a => a.StatTrak == true);

		public string ToMessage(bool? statTrak = false, double? price = null, float? fl = null)
		{
			var stat = StatTrakable && (statTrak ?? false);
			string priceTxt = null;
			if (price != null)
			{
				priceTxt = $"Seller's price: {price.Value.ToString("F", CultureInfo.InvariantCulture)}$";
			}

			string priceTexts;
			if (fl is 0 or null && IsFloated && StatTrakable)
			{
				var prices = GetPrices();
				priceTexts = $"{prices.Min().ToString("F", CultureInfo.InvariantCulture)}$ - {prices.Max().ToString("F", CultureInfo.InvariantCulture)}$";
			}
			else
			{
				priceTexts = $"{GetPrice(fl, stat)?.Value.ToString("F", CultureInfo.InvariantCulture)}$";
			}


			return $"*{(stat ? Helper.StatTrak : String.Empty)} {SearchName}*\n{(fl == null ? String.Empty : Helper.GetFloatName((float) fl))}\nSteam Market Price: {priceTexts}\n\n{priceTxt}";
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