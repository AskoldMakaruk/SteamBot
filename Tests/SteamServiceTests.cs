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
			//var item = await steamService.GetItem("Overgrowth", 0.16f);
			//Assert.AreEqual("USP-S", item.Skin.WeaponName);
		}
	}
}