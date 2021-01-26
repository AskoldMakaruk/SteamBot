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

		public static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
		public static DateTimeOffset DefaultCacheOffset => DateTimeOffset.Now.Add(new TimeSpan(0, 10, 0));

		public Account this[long key]
		{
			get => GetAccount(key);
			set => cache.Set(key, value, DefaultCacheOffset);
		}

		public static Account GetAccount(long chatId)
		{
			var cachedAccount = cache.Get(chatId);
			return cachedAccount as Account;
		}

		public Account GetAccount(Message message)
		{
			var result = GetAccount(message.From.Id);
			if (result != null)
				return result;

			var account = Accounts.FirstOrDefault(a => a.ChatId == message.From.Id) ?? CreateAccount(message);

			cache.Set((long) message.From.Id, account, DefaultCacheOffset);

			return account;
		}

		private Account CreateAccount(Message message)
		{
			var account = new Account
			{
				ChatId = message.Chat.Id,
				Username = message.Chat.Username
			};
			if (message.Chat.Username == null)
				account.Username = message.Chat.FirstName + " " + message.Chat.LastName;
			Accounts.Add(account);
			SaveChanges();
			return account;
		}

		#endregion
	}
}