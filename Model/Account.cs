using System;
using System.Collections;
using System.Collections.Generic;

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
		public bool IsAdmin { get; set; }

		public ICollection<Trade> Trades { get; set; }
	}

	public class Trade
	{
		public int Id { get; set; }
		public long ChannelPostId { get; set; }
		public Item Item { get; set; }
		public Account Buyer { get; set; }
		public Account Seller { get; set; }
		public TradeStatus Status { get; set; }
	}

	public enum TradeStatus
	{
		Open,
		PriceConfirmation,
		PaymentConfirmation,
		Closed,
	}

	public class Item
	{
		public int Id { get; set; }
		public string WeaponName { get; set; }
		public bool IsKnife { get; set; }
		public bool IsStatTrak { get; set; }
		public string SkinName { get; set; }
		public float Float { get; set; }
		public double MarketPrice { get; set; }
		public double Price { get; set; }
		public Image Image { get; set; }

		public string HashName => $"{(IsKnife ? Helper.Star : String.Empty)} {(IsStatTrak ? Helper.StatTrak : String.Empty)} {WeaponName} | {SkinName} ({Helper.GetFloatName(Float)})";
	}

	public class Image
	{
		public int Id { get; set; }
		public byte[] Bytes { get; set; }
		public string FileId { get; set; }
	}
}