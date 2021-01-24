using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Ninject;
using NUnit.Framework;
using SteamBot;
using SteamBot.Services;

namespace Tests
{
	public class SteamServiceTests
	{
		private SteamService steamService;

		[SetUp]
		public void Setup()
		{
			var kernel = new StandardKernel(new Injector());
			kernel.Load(typeof(Program).Assembly);
			steamService = kernel.Get<SteamService>();
		}

		[Test]
		public async Task SteamServiceTest()
		{
			var item = await steamService.GetItem("Overgrowth", 0.16f);
			Assert.AreEqual("USP-S", item.WeaponName);
		}
	}

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
}