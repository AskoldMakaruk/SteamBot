using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SteamBot.Localization;

namespace SteamBot.Model
{
	public class Account
	{
		public int Id { get; set; }
		public long ChatId { get; set; }
		public string Username { get; set; }
		public string SteamId { get; set; }
		public string TradeUrl { get; set; }
		public string Locale { get; set; }
		public virtual bool IsAdmin { get; set; }

		public virtual ICollection<Trade> Trades { get; set; }
	}

	public class Trade
	{
		public int Id { get; set; }
		public long ChannelPostId { get; set; }
		public virtual TradeItem TradeItem { get; set; }
		public virtual Account Buyer { get; set; }
		public virtual Account Seller { get; set; }
		public virtual TradeStatus Status { get; set; }
	}

	public enum TradeStatus
	{
		Open,
		PriceConfirmation,
		PaymentConfirmation,
		Closed,
	}

	public class TradeItem
	{
		public int Id { get; set; }
		public float Float { get; set; }
		public double Price { get; set; }
		public virtual Skin Skin { get; set; }

		public Image Image => Skin.GetImage(Float);

		public string HashName => Skin.GetHashName(Float);
		public double? MarketPrice => Skin.GetMarketPrice(Float);
	}


	public class Skin
	{
		public int Id { get; set; }
		public string WeaponName { get; set; }
		public bool IsKnife { get; set; }
		public bool IsStatTrak { get; set; }
		public string SkinName { get; set; }
		public string SearchName { get; set; }
		public virtual SteamItem SteamItem { get; set; }
		public virtual int SteamItemId { get; set; }
		public bool IsFloated { get; set; }

		public double? Price { get; set; }
		public double? BattleScarredPrice { get; set; }
		public double? FactoryNewPrice { get; set; }
		public double? FieldTestedPrice { get; set; }
		public double? MinimalWearPrice { get; set; }
		public double? WellWornPrice { get; set; }

		public virtual Image Image { get; set; }
		public virtual Image BattleScarredImage { get; set; }
		public virtual Image FactoryNewImage { get; set; }
		public virtual Image FieldTestedImage { get; set; }
		public virtual Image MinimalWearImage { get; set; }
		public virtual Image WellWornImage { get; set; }

		public DateTime CreateTS { get; set; }
		public DateTime UpdateTS { get; set; }

		public virtual ICollection<TradeItem> TradeItems { get; set; }

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
				result.Add((double)BattleScarredPrice);
			}

			if (FactoryNewPrice != null)
			{
				result.Add((double)FactoryNewPrice);
			}

			if (FieldTestedPrice != null)
			{
				result.Add((double)FieldTestedPrice);
			}

			if (MinimalWearPrice != null)
			{
				result.Add((double)MinimalWearPrice);
			}

			if (WellWornPrice != null)
			{
				result.Add((double)WellWornPrice);
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

		public string ToMarkupString(float fl = default)
		{
			return $"*{GetHashName(fl)}*\nPrice: {GetMarketPrice(fl)}$";
		}

		public List<string> GetFloats(string culture)
		{
			if (!IsFloated) return null;
			List<string> result = new();
			var resourceManager = Texts.ResourceManager;
			var c = CultureInfo.GetCultureInfo(culture);

			if (BattleScarredPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Battle_Scarred", c));
			}

			if (FactoryNewPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Factory_New", c));
			}

			if (FieldTestedPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Field_Tested", c));
			}

			if (MinimalWearPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Minimal_Wear", c));
			}

			if (WellWornPrice != null)
			{
				result.Add(resourceManager.GetString("Float_Well_Worn", c));
			}

			return result;
		}
	}

	public class SteamItem
	{
		public int Id { get; set; }
		public virtual Skin Skin { get; set; }
		public virtual int SkinId { get; set; }
		public string Json { get; set; }
	}

	public class Image
	{
		public int Id { get; set; }
		public byte[] Bytes { get; set; }
		public string FileId { get; set; }
	}
}