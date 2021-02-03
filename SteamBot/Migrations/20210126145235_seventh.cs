using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
	public partial class seventh : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				"SteamItemId",
				"Skins",
				"integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateTable(
				"SteamItem",
				table => new
				{
					Id = table.Column<int>("integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Json = table.Column<string>("text", nullable: true)
				},
				constraints: table => { table.PrimaryKey("PK_SteamItem", x => x.Id); });

			migrationBuilder.CreateIndex(
				"IX_Skins_SteamItemId",
				"Skins",
				"SteamItemId",
				unique: true);

			migrationBuilder.CreateIndex(
				"IX_SteamItem_Id",
				"SteamItem",
				"Id");

			migrationBuilder.AddForeignKey(
				"FK_Skins_SteamItem_SteamItemId",
				"Skins",
				"SteamItemId",
				"SteamItem",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Skins_SteamItem_SteamItemId",
				"Skins");

			migrationBuilder.DropTable(
				"SteamItem");

			migrationBuilder.DropIndex(
				"IX_Skins_SteamItemId",
				"Skins");

			migrationBuilder.DropColumn(
				"SteamItemId",
				"Skins");
		}
	}
}