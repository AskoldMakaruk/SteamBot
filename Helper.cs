namespace SteamBot
{
	public class Helper
	{
		public const string Star = "★";
		public const string StatTrak = "StatTrak™";

		public static string GetFloatName(float value)
		{
			if (value >= 0 && value <= 0.07)
			{
				return "Factory New";
			}

			if (value <= 0.15)
			{
				return "Minimal Wear";
			}

			if (value <= 0.38)
			{
				return "Field-Tested";
			}

			if (value <= 0.45)
			{
				return "Well-Worn";
			}

			return "Battle-Scarred";
		}
	}
}
