using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SteamBot.Model;
using Telegram.Bot.Types;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SteamBot.Database
{
	public class TelegramContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Image> Images { get; set; }
		public DbSet<TradeItem> Items { get; set; }
		public DbSet<Skin> Skins { get; set; }
		public DbSet<Trade> Trades { get; set; }
		public DbSet<SteamItem> SteamItems { get; set; }
		
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var configuration = Program.GetConfiguration();
			optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
			optionsBuilder.UseLazyLoadingProxies();

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>(builder =>
			{
				builder.HasMany(a => a.Trades)
					.WithOne(a => a.Seller);

				builder.HasMany(a => a.Buys)
					.WithOne(a => a.Buyer);
			});

			modelBuilder.Entity<Skin>(builder =>
			{
				builder.HasAlternateKey(c => new {c.SkinName, c.WeaponName}).HasName("IX_Fullname");
				builder.HasMany(a => a.TradeItems).WithOne(a => a.Skin);

				builder.Property(a => a.CreateTS)
					.HasDefaultValueSql("NOW()")
					.ValueGeneratedOnAdd();

				builder.Property(a => a.UpdateTS)
					.HasDefaultValueSql("NOW()")
					.ValueGeneratedOnAddOrUpdate();
				builder.HasOne(a => a.SteamItem)
					.WithOne(a => a.Skin)
					.HasForeignKey<SteamItem>(a => a.SkinId);
			});


			base.OnModelCreating(modelBuilder);
		}

		public async Task<Image> GetImage(Skin skin, float fl, string url = null)
		{
			var image = skin.GetImage(fl);
			if (image != null || url == null)
			{
				return image;
			}

			image = new Image();
			skin.SetImage(image, fl);
			using var client = new WebClient();
			image.Bytes = await client.DownloadDataTaskAsync(new Uri(url));
			await Images.AddAsync(image);
			await SaveChangesAsync();

			return image;
		}

		#region Account

		public Account GetAccount(User user) => Accounts.FirstOrDefault(a => a.ChatId == user.Id) ?? CreateAccount(user);

		public Account GetAccount(Message message) => GetAccount(message.From);
		

		private Account CreateAccount(User user)
		{
			var account = new Account
			{
				ChatId = user.Id,
				Username = user.Username
			};
			if (user.Username == null)
				account.Username = user.FirstName + " " + user.LastName;
			Accounts.Add(account);
			SaveChanges();
			return account;
		}

		#endregion
	}
}