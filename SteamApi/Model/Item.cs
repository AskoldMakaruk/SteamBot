using System.Collections.Generic;
using Newtonsoft.Json;

namespace SteamApi.Model
{
	public class Application
	{
		[JsonProperty("appid")]
		public int Appid { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("icon")]
		public string Icon { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }
	}

	public class AppContextData
	{
		[JsonProperty]
		public Application Application { get; set; }
	}

	public class SellOrderArray
	{
		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("quantity")]
		public int Quantity { get; set; }
	}

	public class SellOrderSummary
	{
		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("quantity")]
		public int Quantity { get; set; }
	}

	public class BuyOrderArray
	{
		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("quantity")]
		public int Quantity { get; set; }
	}

	public class BuyOrderSummary
	{
		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("quantity")]
		public int Quantity { get; set; }
	}

	public class Histogram
	{
		[JsonProperty("sell_order_array")]
		public List<SellOrderArray> SellOrderArray { get; set; }

		[JsonProperty("sell_order_summary")]
		public SellOrderSummary SellOrderSummary { get; set; }

		[JsonProperty("buy_order_array")]
		public List<BuyOrderArray> BuyOrderArray { get; set; }

		[JsonProperty("buy_order_summary")]
		public BuyOrderSummary BuyOrderSummary { get; set; }

		[JsonProperty("highest_buy_order")]
		public double HighestBuyOrder { get; set; }

		[JsonProperty("lowest_sell_order")]
		public double LowestSellOrder { get; set; }

		[JsonProperty("buy_order_graph")]
		public List<List<object>> BuyOrderGraph { get; set; }

		[JsonProperty("sell_order_graph")]
		public List<List<object>> SellOrderGraph { get; set; }

		[JsonProperty("graph_max_y")]
		public int GraphMaxY { get; set; }

		[JsonProperty("graph_min_x")]
		public double GraphMinX { get; set; }

		[JsonProperty("graph_max_x")]
		public double GraphMaxX { get; set; }

		[JsonProperty("price_prefix")]
		public string PricePrefix { get; set; }

		[JsonProperty("price_suffix")]
		public string PriceSuffix { get; set; }
	}

	public class Description
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		[JsonProperty("color")]
		public string Color { get; set; }
	}

	public class Action
	{
		[JsonProperty("link")]
		public string Link { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}

	public class Assets
	{
		[JsonProperty("descriptions")]
		public List<Description> Descriptions { get; set; }

		[JsonProperty("actions")]
		public List<Action> Actions { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}

	public class Item
	{
		[JsonProperty("nameID")]
		public int NameID { get; set; }

		[JsonProperty("appID")]
		public int AppID { get; set; }

		[JsonProperty("market_name")]
		public string MarketName { get; set; }

		[JsonProperty("market_hash_name")]
		public string MarketHashName { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("image")]
		public string Image { get; set; }

		[JsonProperty("border_color")]
		public string BorderColor { get; set; }

		[JsonProperty("app_context_data")]
		public AppContextData AppContextData { get; set; }

		[JsonProperty("median_avg_prices_15days")]
		public List<List<object>> MedianAvgPrices15days { get; set; }

		[JsonProperty("histogram")]
		public Histogram Histogram { get; set; }

		[JsonProperty("assets")]
		public Assets Assets { get; set; }

		[JsonProperty("updated_at")]
		public long UpdatedAt { get; set; }
	}
}