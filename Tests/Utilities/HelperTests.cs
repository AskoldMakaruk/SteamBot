using System;
using System.Linq;
using NUnit.Framework;
using SteamBot;

namespace Tests.Utilities
{
	public class HelperTests
	{
		private int[][] collection;
		private int[] input;

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
}