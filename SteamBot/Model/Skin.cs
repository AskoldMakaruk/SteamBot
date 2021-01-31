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
	}
}