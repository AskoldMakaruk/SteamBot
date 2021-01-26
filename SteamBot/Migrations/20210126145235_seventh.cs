using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
    public partial class seventh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SteamItemId",
                table: "Skins",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SteamItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Json = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skins_SteamItemId",
                table: "Skins",
                column: "SteamItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SteamItem_Id",
                table: "SteamItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skins_SteamItem_SteamItemId",
                table: "Skins",
                column: "SteamItemId",
                principalTable: "SteamItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skins_SteamItem_SteamItemId",
                table: "Skins");

            migrationBuilder.DropTable(
                name: "SteamItem");

            migrationBuilder.DropIndex(
                name: "IX_Skins_SteamItemId",
                table: "Skins");

            migrationBuilder.DropColumn(
                name: "SteamItemId",
                table: "Skins");
        }
    }
}
