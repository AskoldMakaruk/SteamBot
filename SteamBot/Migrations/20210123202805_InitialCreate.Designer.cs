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
    [DbContext(typeof(Database))]
    [Migration("20210123202805_InitialCreate")]
    partial class InitialCreate
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

            modelBuilder.Entity("SteamBot.Model.TradeItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TradeItem");
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

                    b.Property<int?>("ItemId")
                        .HasColumnType("integer");

                    b.Property<int?>("SellerId")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.HasIndex("ItemId");

                    b.HasIndex("SellerId");

                    b.ToTable("Trade");
                });

            modelBuilder.Entity("SteamBot.Model.Trade", b =>
                {
                    b.HasOne("SteamBot.Model.Account", "Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerId");

                    b.HasOne("SteamBot.Model.TradeItem", "TradeItem")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.HasOne("SteamBot.Model.Account", "Seller")
                        .WithMany("Trades")
                        .HasForeignKey("SellerId");

                    b.Navigation("Buyer");

                    b.Navigation("TradeItem");

                    b.Navigation("Seller");
                });

            modelBuilder.Entity("SteamBot.Model.Account", b =>
                {
                    b.Navigation("Trades");
                });
#pragma warning restore 612, 618
        }
    }
}
