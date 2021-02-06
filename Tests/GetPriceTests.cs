using System.Collections.Generic;
using NUnit.Framework;
using SteamBot;
using SteamBot.Model;

namespace Tests
{
	public class GetPriceTests
	{
		private Skin skin;
		private List<SkinPrice> prices;

		[SetUp]
		public void Setup()
		{
			prices = new();
			skin = new Skin()
			{
				Prices = prices
			};

			prices.AddRange(
				new[]
				{
					new SkinPrice
					{
						StatTrak = false,
						Float = 0.06f,
						Value = 100
					},
					new SkinPrice
					{
						StatTrak = true,
						Float = 0.06f,
						Value = 200
					},
					new SkinPrice
					{
						StatTrak = false,
						Float = 0.30f,
						Value = 10
					},
					new SkinPrice
					{
						StatTrak = true,
						Float = 0.30f,
						Value = 20
					}
				});
			foreach (var price in prices)
			{
				price.FloatName = Helper.GetFloatName((float) price.Float);
			}
		}

		[Test]
		public void GetPrice()
		{
			Assert.AreEqual(100d, skin.GetPrice(0.06f, false).Value);
			Assert.AreEqual(200d, skin.GetPrice(0.06f, true).Value);
			Assert.AreEqual(10d, skin.GetPrice(0.30f, false).Value);
			Assert.AreEqual(20d, skin.GetPrice(0.30f, true).Value);
		}
	}
}