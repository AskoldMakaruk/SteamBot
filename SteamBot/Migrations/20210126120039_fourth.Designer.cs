﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SteamBot.Services;

namespace SteamBot.Migrations
{
    [DbContext(typeof(TelegramContext))]
    [Migration("20210126120039_fourth")]
    partial class fourth
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("SteamBot.Model.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("Locale")
                        .HasColumnType("text");

                    b.Property<string>("SteamId")
                        .HasColumnType("text");

                    b.Property<string>("TradeUrl")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("SteamBot.Model.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<byte[]>("Bytes")
                        .HasColumnType("bytea");

                    b.Property<string>("FileId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("SteamBot.Model.Skin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int?>("BattleScarredImageId")
                        .HasColumnType("integer");

                    b.Property<double?>("BattleScarredPrice")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreateTS")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<int?>("FactoryNewImageId")
                        .HasColumnType("integer");

                    b.Property<double?>("FactoryNewPrice")
                        .HasColumnType("double precision");

                    b.Property<int?>("FieldTestedImageId")
                        .HasColumnType("integer");

                    b.Property<double?>("FieldTestedPrice")
                        .HasColumnType("double precision");

                    b.Property<int?>("ImageId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsFloated")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsKnife")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsStatTrak")
                        .HasColumnType("boolean");

                    b.Property<int?>("MinimalWearImageId")
                        .HasColumnType("integer");

                    b.Property<double?>("MinimalWearPrice")
                        .HasColumnType("double precision");

                    b.Property<double?>("Price")
                        .HasColumnType("double precision");

                    b.Property<string>("SkinName")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdateTS")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<string>("WeaponName")
                        .HasColumnType("text");

                    b.Property<int?>("WellWornImageId")
                        .HasColumnType("integer");

                    b.Property<double?>("WellWornPrice")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("BattleScarredImageId");

                    b.HasIndex("FactoryNewImageId");

                    b.HasIndex("FieldTestedImageId");

                    b.HasIndex("ImageId");

                    b.HasIndex("MinimalWearImageId");

                    b.HasIndex("WellWornImageId");

                    b.ToTable("Skins");
                });

            modelBuilder.Entity("SteamBot.Model.Trade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int?>("BuyerId")
                        .HasColumnType("integer");

                    b.Property<long>("ChannelPostId")
                        .HasColumnType("bigint");

                    b.Property<int?>("SellerId")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int?>("TradeItemId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.HasIndex("SellerId");

                    b.HasIndex("TradeItemId");

                    b.ToTable("Trades");
                });

            modelBuilder.Entity("SteamBot.Model.TradeItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<float>("Float")
                        .HasColumnType("real");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<int?>("SkinId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SkinId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("SteamBot.Model.Skin", b =>
                {
                    b.HasOne("SteamBot.Model.Image", "BattleScarredImage")
                        .WithMany()
                        .HasForeignKey("BattleScarredImageId");

                    b.HasOne("SteamBot.Model.Image", "FactoryNewImage")
                        .WithMany()
                        .HasForeignKey("FactoryNewImageId");

                    b.HasOne("SteamBot.Model.Image", "FieldTestedImage")
                        .WithMany()
                        .HasForeignKey("FieldTestedImageId");

                    b.HasOne("SteamBot.Model.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.HasOne("SteamBot.Model.Image", "MinimalWearImage")
                        .WithMany()
                        .HasForeignKey("MinimalWearImageId");

                    b.HasOne("SteamBot.Model.Image", "WellWornImage")
                        .WithMany()
                        .HasForeignKey("WellWornImageId");

                    b.Navigation("BattleScarredImage");

                    b.Navigation("FactoryNewImage");

                    b.Navigation("FieldTestedImage");

                    b.Navigation("Image");

                    b.Navigation("MinimalWearImage");

                    b.Navigation("WellWornImage");
                });

            modelBuilder.Entity("SteamBot.Model.Trade", b =>
                {
                    b.HasOne("SteamBot.Model.Account", "Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerId");

                    b.HasOne("SteamBot.Model.Account", "Seller")
                        .WithMany("Trades")
                        .HasForeignKey("SellerId");

                    b.HasOne("SteamBot.Model.TradeItem", "TradeItem")
                        .WithMany()
                        .HasForeignKey("TradeItemId");

                    b.Navigation("Buyer");

                    b.Navigation("Seller");

                    b.Navigation("TradeItem");
                });

            modelBuilder.Entity("SteamBot.Model.TradeItem", b =>
                {
                    b.HasOne("SteamBot.Model.Skin", "Skin")
                        .WithMany("TradeItems")
                        .HasForeignKey("SkinId");

                    b.Navigation("Skin");
                });

            modelBuilder.Entity("SteamBot.Model.Account", b =>
                {
                    b.Navigation("Trades");
                });

            modelBuilder.Entity("SteamBot.Model.Skin", b =>
                {
                    b.Navigation("TradeItems");
                });
#pragma warning restore 612, 618
        }
    }
}
