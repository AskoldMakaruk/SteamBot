using System;
using System.Linq;
using NUnit.Framework;
using SteamBot;
using Telegram.Bot.Types;

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

	public class GetFloatTests
	{
		[SetUp]
		public void Setup() { }

		[Test]
		public void CollectionsTest()
		{
			var floats = Helper.Floats();
			foreach (var (key, _) in floats)
			{
				var fl = new Message {Text = $"1\n{key}"}.GetFloat();
				Console.WriteLine($"Parsed values: {fl}");
				var input = Helper.GetFloatName((float) fl);
				Console.WriteLine(key);
				Assert.AreEqual(input, key);
			}
		}
	}
}