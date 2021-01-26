using System;
using System.Linq;
using NUnit.Framework;
using SteamBot;

namespace Tests
{
	public class HelperTests
	{
		private int[] input;

		private int[][] collection;

		[SetUp]
		public void Setup()
		{
			input = Enumerable.Range(1, 5).ToArray();
			collection = new[]
			{
				new[]
				{
					1, 2
				},
				new[]
				{
					3, 4
				},
				new[]
				{
					5
				}
			};
		}

		[Test]
		public void CollectionsTest()
		{
			var result = input.GroupElements(2).Select(a => a.ToArray()).ToArray();
			foreach (var (first, second) in result.Zip(collection).ToArray())
			{
				Assert.True(first.SequenceEqual(second));
			}
		}

		[Test]
		public void StupidTest()
		{
			var result = input.GroupBy(a => Array.IndexOf(input, a) / 2).Select(a => a.ToArray()).ToArray();
			foreach (var (first, second) in result.Zip(collection).ToArray())
			{
				Assert.True(first.SequenceEqual(second));
			}
		}

	}

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