using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SteamBot
{
	public static class ImageHelper
	{
		public static async Task<byte[]> ProcessImage(byte[] imageBytes)
		{
			var backImg = await Image.LoadAsync(@"background.png");
			var skinImg = Image.Load(imageBytes);	

			var size = skinImg.Size();
			skinImg.Mutate(a => a.Resize(size * 360 / size.Width));

			backImg.Mutate(a =>
			{
				var backSize = a.GetCurrentSize();
				a.DrawImage(skinImg, new Point(40, (backSize.Height - skinImg.Height) / 2), 1);
			});

			await using var memoryStream = new MemoryStream();
			await backImg.SaveAsPngAsync(memoryStream);

			backImg.Dispose();
			skinImg.Dispose();

			return memoryStream.ToArray();
			//await backImg.SaveAsPngAsync("image.png");
		}
	}
}