using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ninject.Activation;
using SteamBot.Model;
using Telegram.Bot.Types;
using System.Linq;

namespace SteamBot.Database
{
	public class TelegramContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Image> Images { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Trade> Trades { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=64.227.79.234;Port=5432;Database=SteamBot;Username=askold;Password=139742685Aa");

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>(builder =>
			{
				builder
					.HasMany(a => a.Trades)
					.WithOne(a => a.Seller);
			});

			base.OnModelCreating(modelBuilder);
		}

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
	}
}