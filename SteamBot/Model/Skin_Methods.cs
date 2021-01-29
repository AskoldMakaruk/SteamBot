using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SteamBot.Localization;

namespace SteamBot.Model
{
	public partial class Skin
	{
		public string GetHashName(float fl) => GetHashName(Helper.GetFloatName(fl));
		public string GetHashName(string fl) => $"{GetHashName()} {(IsFloated ? $"({fl})" : String.Empty)}";
		private string GetHashName() => $"{(IsKnife ? Helper.Star : String.Empty)} {(IsStatTrak ? Helper.StatTrak : String.Empty)} {WeaponName} | {SkinName}";

		public List<double> GetPrices()
		{
			if (!IsFloated)
			{
				return new List<double>
				{
					(double) Price
				};
			}

			List<double> result = new();


			if (BattleScarredPrice != null)
			{
				result.Add((double) BattleScarredPrice);
			}

			if (FactoryNewPrice != null)
			{
				result.Add((double) FactoryNewPrice);
			}

			if (FieldTestedPrice != null)
			{
				result.Add((double) FieldTestedPrice);
			}

			if (MinimalWearPrice != null)
			{
				result.Add((double) MinimalWearPrice);
			}

			if (WellWornPrice != null)
			{
				result.Add((double) WellWornPrice);
			}

			return result;
		}

		public Image GetImage(float fl = default)
		{
			if (!IsFloated) return Image;
			if (fl >= 0 && fl <= 0.07)
			{
				return FactoryNewImage;
			}

			if (fl <= 0.15)
			{
				return MinimalWearImage;
			}

			if (fl <= 0.38)
			{
				return FieldTestedImage;
			}

			if (fl <= 0.45)
			{
				return WellWornImage;
			}

			return BattleScarredImage;
		}

		public void SetImage(Image image, float fl)
		{
			if (!IsFloated) Image = image;
			if (fl >= 0 && fl <= 0.07)
			{
				FactoryNewImage = image;
			}

			else if (fl <= 0.15)
			{
				MinimalWearImage = image;
			}

			else if (fl <= 0.38)
			{
				FieldTestedImage = image;
			}

			else if (fl <= 0.45)
			{
				WellWornImage = image;
			}

			else BattleScarredImage = image;
		}

		public double? GetMarketPrice(float fl = default)
		{
			if (!IsFloated) return Price;
			if (fl >= 0 && fl <= 0.07)
			{
				return FactoryNewPrice;
			}

			if (fl <= 0.15)
			{
				return MinimalWearPrice;
			}

			if (fl <= 0.38)
			{
				return FieldTestedPrice;
			}

			if (fl <= 0.45)
			{
				return WellWornPrice;
			}

			return BattleScarredPrice;
		}

		public void ParseHashName(string hashName)
		{
			IsFloated = Helper.IsFloated(hashName);
			var HasDelimiter = hashName.Contains('|');
			var marketHashName = hashName;

			IsKnife = marketHashName.Contains(Helper.Star);
			if (IsKnife)
			{
				marketHashName = marketHashName.Replace(Helper.Star, String.Empty).Trim();
			}

			IsStatTrak = marketHashName.Contains(Helper.StatTrak);
			if (IsStatTrak)
			{
				marketHashName = marketHashName.Replace(Helper.StatTrak, String.Empty).Trim();
			}

			var delimiterIndx = marketHashName.IndexOf('|');

			WeaponName = HasDelimiter ? marketHashName[..delimiterIndx].Trim() : marketHashName.Trim();

			if (HasDelimiter)
			{
				SkinName = (IsFloated ? marketHashName[(delimiterIndx + 2)..marketHashName.IndexOf('(')] : marketHashName[(delimiterIndx + 2)..]).Trim();
			}
			else
			{
				SkinName = WeaponName;
			}

			SearchName = $"{WeaponName} {SkinName}";
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

		public string ToMessage(float fl = default)
		{
			var price = GetMarketPrice(fl)?.ToString("F", CultureInfo.InvariantCulture);
			return $"*{SearchName}*\n{Helper.GetFloatName(fl)}\nPrice: {price}$";
		}

		public static Skin FromMessage()
		{
			return null;
		}

		public List<string> GetFloats(string culture)
		{
			if (!IsFloated) return null;
			List<string> result = new();
			var resourceManager = Texts.ResourceManager;
			var c = CultureInfo.GetCultureInfo(culture);

			if (FactoryNewPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Factory_New", c));
			}

			if (MinimalWearPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Minimal_Wear", c));
			}
			
			if (FieldTestedPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Field_Tested", c));
			}

			if (WellWornPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Well_Worn", c));
			}

			if (BattleScarredPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Battle_Scarred", c));
			}

			return result;
		}
	}
}