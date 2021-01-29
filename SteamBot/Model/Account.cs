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