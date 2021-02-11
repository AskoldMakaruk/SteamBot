using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SteamBot.Services;

namespace Tests.Services
{
	public class SteamServiceTests
	{
		private SteamService steamService;

		[SetUp]
		public void Setup()
		{
			var setup = new TestSetup();
			steamService = setup.AppHost.Services.GetService<SteamService>();
		}

		[Test]
		public async Task SteamServiceTest()
		{
			Assert.DoesNotThrowAsync(async () => await steamService.UpdateDb());
			//var item = await steamService.GetSteamItem("Overgrowth", 0.16f);
			//Assert.AreEqual("USP-S", item.Skin.WeaponName);
		}
	}
}