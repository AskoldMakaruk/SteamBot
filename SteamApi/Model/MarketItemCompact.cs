using System;
using System.Collections.Generic;

namespace SteamApi.Model
{
	public record MarketItemCompact(double Price, string Name);

	public record MarketCompact(List<MarketItemCompact> Items);
}