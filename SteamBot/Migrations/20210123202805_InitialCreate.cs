using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				"Accounts",
				table => new
				{
					Id = table.Column<int>("integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					ChatId = table.Column<long>("bigint", nullable: false),
					Username = table.Column<string>("text", nullable: true),
					SteamId = table.Column<string>("text", nullable: true),
					TradeUrl = table.Column<string>("text", nullable: true),
					Locale = table.Column<string>("text", nullable: true)
				},
				constraints: table => { table.PrimaryKey("PK_Accounts", x => x.Id); });

			migrationBuilder.CreateTable(
				"TradeItem",
				table => new
				{
					Id = table.Column<int>("integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>("text", nullable: true)
				},
				constraints: table => { table.PrimaryKey("PK_Item", x => x.Id); });

			migrationBuilder.CreateTable(
				"Trade",
				table => new
				{
					Id = table.Column<int>("integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					ChannelPostId = table.Column<long>("bigint", nullable: false),
					ItemId = table.Column<int>("integer", nullable: true),
					BuyerId = table.Column<int>("integer", nullable: true),
					SellerId = table.Column<int>("integer", nullable: true),
					Status = table.Column<int>("integer", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Trade", x => x.Id);
					table.ForeignKey(
						"FK_Trade_Accounts_BuyerId",
						x => x.BuyerId,
						"Accounts",
						"Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						"FK_Trade_Accounts_SellerId",
						x => x.SellerId,
						"Accounts",
						"Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						"FK_Trade_Item_ItemId",
						x => x.ItemId,
						"TradeItem",
						"Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				"IX_Trade_BuyerId",
				"Trade",
				"BuyerId");

			migrationBuilder.CreateIndex(
				"IX_Trade_ItemId",
				"Trade",
				"ItemId");

			migrationBuilder.CreateIndex(
				"IX_Trade_SellerId",
				"Trade",
				"SellerId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				"Trade");

			migrationBuilder.DropTable(
				"Accounts");

			migrationBuilder.DropTable(
				"TradeItem");
		}
	}
}