using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using SteamApi.Model;

namespace SteamApi
{
	public class SteamApiClient : IDisposable
	{
		public string Token { get; }

		private readonly HttpClient client;

		/// <summary>
		/// Creates new SteamApiClient.
		/// </summary>
		/// <param name="token">Can be generated in Discord with bot BrawlAPI#8520</param>
		public SteamApiClient(string token)
		{
			Token = token;
			client = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(10),
				BaseAddress = new Uri(Utils.Base)
			};
		}


		/// <summary>
		/// Gets item from steam market.
		/// </summary>
		/// <param name="appId">App id from https://steamdb.info/apps/ </param>
		/// <param name="itemHashName">Full item name in string</param>
		/// <returns>Item from market.</returns>
		public async Task<Item> GetItem(string appId, string itemHashName)
		{
			
			var response = await client.GetAsync($"{Utils.Item}/{appId}/{itemHashName}?api_key={Token}");

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			var json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<Item>(json);
		}

		public async Task<Item> GetCSGOItem(string itemHashName) => await GetItem(AppId.CSGO, itemHashName);

		public void Dispose()
		{
			client.Dispose();
		}
	}
}