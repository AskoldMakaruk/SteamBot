using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
	public partial class chat_rooms : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				"ChatRooms",
				table => new
				{
					Id = table.Column<int>("integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					InviteLink = table.Column<string>("text", nullable: true),
					ChatId = table.Column<long>("bigint", nullable: false),
					TradeId = table.Column<int>("integer", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ChatRooms", x => x.Id);
					table.ForeignKey(
						"FK_ChatRooms_Trades_TradeId",
						x => x.TradeId,
						"Trades",
						"Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				"IX_ChatRooms_TradeId",
				"ChatRooms",
				"TradeId",
				unique: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				"ChatRooms");
		}
	}
}