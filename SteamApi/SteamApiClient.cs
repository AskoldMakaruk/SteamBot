using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

		public async Task<Item> GetCSGOItem(string itemHashName) => await GetItem(AppId.CSGO, itemHashName);

		public async Task<MarketCompact> GetCSGOItems() => await GetMarketItems(AppId.CSGO);


		/// <summary>
		/// Gets market items.
		/// </summary>
		/// <param name="appId"></param>
		/// <returns></returns>
		public async Task<MarketCompact> GetMarketItems(string appId)
		{
			var url = $"{Utils.Items}/{appId}?api_key={Token}&format=compact";
			var result = await Get(url);
			var json = JObject.Parse(result);
			var list = json.Children()
				.OfType<JProperty>()
				.Select(a => new MarketItemCompact(a.Value.Value<double>(), a.Name))
				.ToList();
			return new MarketCompact(list);
		}

		/// <summary>
		/// Gets item from steam market.
		/// </summary>
		/// <param name="appId">App id from https://steamdb.info/apps/ </param>
		/// <param name="itemHashName">Full item name in string</param>
		/// <returns>Item from market.</returns>
		public async Task<Item> GetItem(string appId, string itemHashName) => await Get<Item>($"{Utils.Item}/{appId}/{itemHashName.Trim()}?api_key={Token}");


		private async Task<T> Get<T>(string url)
		{
			var result = await Get(url);
			return result != null ? JsonConvert.DeserializeObject<T>(result) : default;
		}

		private async Task<string> Get(string url)
		{
			var response = await client.GetAsync(url);
			return !response.IsSuccessStatusCode ? default : await response.Content.ReadAsStringAsync();
		}


		public void Dispose()
		{
			client.Dispose();
		}
	}
}