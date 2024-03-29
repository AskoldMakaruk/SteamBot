﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SteamBot.Model;
using Telegram.Bot.Types;

namespace SteamBot.Services
{

	//uncomment in case of a migration
	//public class AppDbContextFactory : IDesignTimeDbContextFactory<Database>
	//{
	//	public Database CreateDbContext(string[] args)
	//	{
	//		var optionsBuilder = new DbContextOptionsBuilder<Database>();
	//		var _configuration = Program.GetConfiguration();
	//		optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
	//		optionsBuilder.UseLazyLoadingProxies();

	//		return new Database(optionsBuilder.Options);
	//	}
	//}

	public class Database : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Image> Images { get; set; }
		public DbSet<Skin> Skins { get; set; }
		public DbSet<Trade> Trades { get; set; }
		public DbSet<SteamItem> SteamItems { get; set; }
		public DbSet<ChatRoom> ChatRooms { get; set; }
		public DbSet<Translation> Translations { get; set; }

		public Database(DbContextOptions<Database> options) : base(options) { }

		public Database() { }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//var _configuration = Program.GetConfiguration();
			//optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
			//optionsBuilder.UseLazyLoadingProxies();
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
				builder.HasMany(a => a.Trades).WithOne(a => a.Skin);

				builder.HasMany(a => a.Prices)
					.WithOne(a => a.Skin);

				builder.HasOne(a => a.SteamItem)
					.WithOne(a => a.Skin)
					.HasForeignKey<SteamItem>(a => a.SkinId);

				builder.Property(a => a.UpdateTS)
					.HasDefaultValueSql("NOW()")
					.ValueGeneratedOnAddOrUpdate();
				builder.Property(a => a.CreateTS)
					.HasDefaultValueSql("NOW()")
					.ValueGeneratedOnAdd();
			});

			base.OnModelCreating(modelBuilder);
		}

		public async Task DeleteOldSkins()
		{
			Skins.RemoveRange(Skins.Where(a => a.WeaponName == a.SkinName));
			await SaveChangesAsync();
		}

		public async Task<Image> GetImage(Skin skin, float? fl, string url = null)
		{
			var image = skin.GetImage(fl);
			if (image?.Bytes != null || url == null)
			{
				return image;
			}

			image ??= new Image();

			skin.GetPrice(fl).Image = image;
			using var client = new WebClient();

			image.Bytes = await ImageHelper.ProcessImage(await client.DownloadDataTaskAsync(new Uri(url)));

			//skin.SetImage(image, fl);
			await Images.AddAsync(image);
			await SaveChangesAsync();

			return image;
		}

		public async Task<Skin> GetSkinAsync(CallbackQuery query)
		{
			var split = query.Data.Split(' ');
			if (split.Length == 0 || !Int32.TryParse(split[0].Trim(), out var id))
			{
				return null;
			}

			var skin = await Skins.FindAsync(id);
			return skin;
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
			{
				account.Username = user.FirstName + " " + user.LastName;
			}

			Accounts.Add(account);
			SaveChanges();
			return account;
		}

		#endregion
	}
}