using System;
using System.Collections.Generic;

namespace SteamBot.Model
{
	public partial class Skin
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

		public DateTime CreateTS { get; set; }
		public DateTime UpdateTS { get; set; }

		public virtual ICollection<Trade> Trades { get; set; } = new HashSet<Trade>();
		public virtual ICollection<SkinPrice> Prices { get; set; } = new HashSet<SkinPrice>();
	}

	public class SkinPrice
	{
		public int Id { get; set; }
		public int SkinId { get; set; }
		public bool? StatTrak { get; set; }
		public float? Float { get; set; }
		public string FloatName { get; set; }
		public double Value { get; set; }

		public virtual Image Image { get; set; }
		public virtual Skin Skin { get; set; }
	}
}