using System;
using NUnit.Framework;
using SteamBot;
using Telegram.Bot.Types;

namespace Tests.Utilities
{
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