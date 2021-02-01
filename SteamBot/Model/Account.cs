using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

		public virtual ICollection<Trade> Trades { get; set; } = new HashSet<Trade>();
		public virtual ICollection<Trade> Buys { get; set; } = new HashSet<Trade>();

		[NotMapped]
		public TradeItem CurrentTrade { get; set; }
	}

	public class Trade
	{
		public int Id { get; set; }
		public long ChannelPostId { get; set; }
		public virtual ChatRoom Room { get; set; }
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
		public double? MarketPrice => Skin.GetPrice(Float).Value;
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

	public class ChatRoom
	{
		public int Id { get; set; }
		public string InviteLink { get; set; }
		public long ChatId { get; set; }
		public int? TradeId { get; set; }
		public bool AllMembersInside { get; set; }
		public DateTime LastMemberChange { get; set; }

		public virtual Trade Trade { get; set; }
	}
}