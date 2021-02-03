using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
	public partial class second : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Trade_Accounts_BuyerId",
				"Trade");

			migrationBuilder.DropForeignKey(
				"FK_Trade_Accounts_SellerId",
				"Trade");

			migrationBuilder.DropForeignKey(
				"FK_Trade_Item_ItemId",
				"Trade");

			migrationBuilder.DropPrimaryKey(
				"PK_Trade",
				"Trade");

			migrationBuilder.DropPrimaryKey(
				"PK_Item",
				"TradeItem");

			migrationBuilder.RenameTable(
				"Trade",
				newName: "Trades");

			migrationBuilder.RenameTable(
				"TradeItem",
				newName: "Items");

			migrationBuilder.RenameIndex(
				"IX_Trade_SellerId",
				table: "Trades",
				newName: "IX_Trades_SellerId");

			migrationBuilder.RenameIndex(
				"IX_Trade_ItemId",
				table: "Trades",
				newName: "IX_Trades_ItemId");

			migrationBuilder.RenameIndex(
				"IX_Trade_BuyerId",
				table: "Trades",
				newName: "IX_Trades_BuyerId");

			migrationBuilder.RenameColumn(
				"Name",
				"Items",
				"WeaponName");

			migrationBuilder.AddColumn<bool>(
				"IsAdmin",
				"Accounts",
				"boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<float>(
				"Float",
				"Items",
				"real",
				nullable: false,
				defaultValue: 0f);

			migrationBuilder.AddColumn<int>(
				"ImageId",
				"Items",
				"integer",
				nullable: true);

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

			migrationBuilder.AddColumn<double>(
				"Price",
				"Items",
				"double precision",
				nullable: false,
				defaultValue: 0.0);

			migrationBuilder.AddColumn<string>(
				"SkinName",
				"Items",
				"text",
				nullable: true);

			migrationBuilder.AddPrimaryKey(
				"PK_Trades",
				"Trades",
				"Id");

			migrationBuilder.AddPrimaryKey(
				"PK_Items",
				"Items",
				"Id");

			migrationBuilder.CreateTable(
				"Images",
				table => new
				{
					Id = table.Column<int>("integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Bytes = table.Column<byte[]>("bytea", nullable: true),
					FileId = table.Column<string>("text", nullable: true)
				},
				constraints: table => { table.PrimaryKey("PK_Images", x => x.Id); });

			migrationBuilder.CreateIndex(
				"IX_Items_ImageId",
				"Items",
				"ImageId");

			migrationBuilder.AddForeignKey(
				"FK_Items_Images_ImageId",
				"Items",
				"ImageId",
				"Images",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Trades_Accounts_BuyerId",
				"Trades",
				"BuyerId",
				"Accounts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Trades_Accounts_SellerId",
				"Trades",
				"SellerId",
				"Accounts",
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

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Items_Images_ImageId",
				"Items");

			migrationBuilder.DropForeignKey(
				"FK_Trades_Accounts_BuyerId",
				"Trades");

			migrationBuilder.DropForeignKey(
				"FK_Trades_Accounts_SellerId",
				"Trades");

			migrationBuilder.DropForeignKey(
				"FK_Trades_Items_ItemId",
				"Trades");

			migrationBuilder.DropTable(
				"Images");

			migrationBuilder.DropPrimaryKey(
				"PK_Trades",
				"Trades");

			migrationBuilder.DropPrimaryKey(
				"PK_Items",
				"Items");

			migrationBuilder.DropIndex(
				"IX_Items_ImageId",
				"Items");

			migrationBuilder.DropColumn(
				"IsAdmin",
				"Accounts");

			migrationBuilder.DropColumn(
				"Float",
				"Items");

			migrationBuilder.DropColumn(
				"ImageId",
				"Items");

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
				"Price",
				"Items");

			migrationBuilder.DropColumn(
				"SkinName",
				"Items");

			migrationBuilder.RenameTable(
				"Trades",
				newName: "Trade");

			migrationBuilder.RenameTable(
				"Items",
				newName: "TradeItem");

			migrationBuilder.RenameIndex(
				"IX_Trades_SellerId",
				table: "Trade",
				newName: "IX_Trade_SellerId");

			migrationBuilder.RenameIndex(
				"IX_Trades_ItemId",
				table: "Trade",
				newName: "IX_Trade_ItemId");

			migrationBuilder.RenameIndex(
				"IX_Trades_BuyerId",
				table: "Trade",
				newName: "IX_Trade_BuyerId");

			migrationBuilder.RenameColumn(
				"WeaponName",
				"TradeItem",
				"Name");

			migrationBuilder.AddPrimaryKey(
				"PK_Trade",
				"Trade",
				"Id");

			migrationBuilder.AddPrimaryKey(
				"PK_Item",
				"TradeItem",
				"Id");

			migrationBuilder.AddForeignKey(
				"FK_Trade_Accounts_BuyerId",
				"Trade",
				"BuyerId",
				"Accounts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Trade_Accounts_SellerId",
				"Trade",
				"SellerId",
				"Accounts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Trade_Item_ItemId",
				"Trade",
				"ItemId",
				"TradeItem",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}