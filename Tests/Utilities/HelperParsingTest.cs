using NUnit.Framework;
using SteamBot;

namespace Tests.Utilities
{
	public class HelperParsingTest
	{
		[Test]
		public void CollectionsTest()
		{
			Assert.AreEqual(Helper.IsFloated("Nova | Plume (Minimal Wear)"), true);
			Assert.AreEqual(Helper.IsFloated("Sticker | GuardiaN (Foil) | Atlanta 2017"), false);
			Assert.AreEqual(Helper.IsFloated("★ StatTrak™ Karambit | Night (Battle-Scarred)"), true);
			Assert.AreEqual(Helper.IsFloated("★ Falchion Knife"), false);
			Assert.AreEqual(Helper.IsFloated("Sticker | k0nfig (Gold) | Krakow 2017"), false);
		}
	}
}