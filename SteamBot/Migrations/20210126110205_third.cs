using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
	public partial class third : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Items_Images_ImageId",
				"Items");

			migrationBuilder.DropForeignKey(
				"FK_Trades_Items_ItemId",
				"Trades");

			migrationBuilder.DropColumn(
				"IsKnife",
				"Items");

			migrationBuilder.DropColumn(
				"IsStatTrak",
				"Items");

			migrationBuilder.DropColumn(
				"MarketPrice",
				"Items");

			migrationBuilder.DropColumn(
				"SkinName",
				"Items");

			migrationBuilder.DropColumn(
				"WeaponName",
				"Items");

			migrationBuilder.RenameColumn(
				"ItemId",
				"Trades",
				"TradeItemId");

			migrationBuilder.RenameIndex(
				"IX_Trades_ItemId",
				table: "Trades",
				newName: "IX_Trades_TradeItemId");

			migrationBuilder.RenameColumn(
				"ImageId",
				"Items",
				"SkinId");

			migrationBuilder.RenameIndex(
				"IX_Items_ImageId",
				table: "Items",
				newName: "IX_Items_SkinId");

			migrationBuilder.CreateTable(
				"Skins",
				table => new
				{
					Id = table.Column<int>("integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					WeaponName = table.Column<string>("text", nullable: true),
					IsKnife = table.Column<bool>("boolean", nullable: false),
					IsStatTrak = table.Column<bool>("boolean", nullable: false),
					SkinName = table.Column<string>("text", nullable: true),
					BattleScarredPrice = table.Column<double>("double precision", nullable: true),
					FactoryNewPrice = table.Column<double>("double precision", nullable: true),
					FieldTestedPrice = table.Column<double>("double precision", nullable: true),
					MinimalWearPrice = table.Column<double>("double precision", nullable: true),
					WellWornPrice = table.Column<double>("double precision", nullable: true),
					BattleScarredImageId = table.Column<int>("integer", nullable: true),
					FactoryNewImageId = table.Column<int>("integer", nullable: true),
					FieldTestedImageId = table.Column<int>("integer", nullable: true),
					MinimalWearImageId = table.Column<int>("integer", nullable: true),
					WellWornImageId = table.Column<int>("integer", nullable: true),
					CreateTS = table.Column<DateTime>("timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
					UpdateTS = table.Column<DateTime>("timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Skins", x => x.Id);
					table.ForeignKey(
						"FK_Skins_Images_BattleScarredImageId",
						x => x.BattleScarredImageId,
						"Images",
						"Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						"FK_Skins_Images_FactoryNewImageId",
						x => x.FactoryNewImageId,
						"Images",
						"Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						"FK_Skins_Images_FieldTestedImageId",
						x => x.FieldTestedImageId,
						"Images",
						"Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						"FK_Skins_Images_MinimalWearImageId",
						x => x.MinimalWearImageId,
						"Images",
						"Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						"FK_Skins_Images_WellWornImageId",
						x => x.WellWornImageId,
						"Images",
						"Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				"IX_Skins_BattleScarredImageId",
				"Skins",
				"BattleScarredImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_FactoryNewImageId",
				"Skins",
				"FactoryNewImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_FieldTestedImageId",
				"Skins",
				"FieldTestedImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_MinimalWearImageId",
				"Skins",
				"MinimalWearImageId");

			migrationBuilder.CreateIndex(
				"IX_Skins_WellWornImageId",
				"Skins",
				"WellWornImageId");

			migrationBuilder.AddForeignKey(
				"FK_Items_Skins_SkinId",
				"Items",
				"SkinId",
				"Skins",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Trades_Items_TradeItemId",
				"Trades",
				"TradeItemId",
				"Items",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Items_Skins_SkinId",
				"Items");

			migrationBuilder.DropForeignKey(
				"FK_Trades_Items_TradeItemId",
				"Trades");

			migrationBuilder.DropTable(
				"Skins");

			migrationBuilder.RenameColumn(
				"TradeItemId",
				"Trades",
				"ItemId");

			migrationBuilder.RenameIndex(
				"IX_Trades_TradeItemId",
				table: "Trades",
				newName: "IX_Trades_ItemId");

			migrationBuilder.RenameColumn(
				"SkinId",
				"Items",
				"ImageId");

			migrationBuilder.RenameIndex(
				"IX_Items_SkinId",
				table: "Items",
				newName: "IX_Items_ImageId");

			migrationBuilder.AddColumn<bool>(
				"IsKnife",
				"Items",
				"boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				"IsStatTrak",
				"Items",
				"boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<double>(
				"MarketPrice",
				"Items",
				"double precision",
				nullable: false,
				defaultValue: 0.0);

			migrationBuilder.AddColumn<string>(
				"SkinName",
				"Items",
				"text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				"WeaponName",
				"Items",
				"text",
				nullable: true);

			migrationBuilder.AddForeignKey(
				"FK_Items_Images_ImageId",
				"Items",
				"ImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Trades_Items_ItemId",
				"Trades",
				"ItemId",
				"Items",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}