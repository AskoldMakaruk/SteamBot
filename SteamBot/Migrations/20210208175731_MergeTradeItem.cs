using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SteamBot.Migrations
{
    public partial class MergeTradeItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkinPrice_Skins_SkinId",
                table: "SkinPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Items_TradeItemId",
                table: "Trades");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.RenameColumn(
                name: "TradeItemId",
                table: "Trades",
                newName: "SkinId");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_TradeItemId",
                table: "Trades",
                newName: "IX_Trades_SkinId");

            migrationBuilder.AddColumn<double>(
                name: "BuyerPrice",
                table: "Trades",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Float",
                table: "Trades",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<double>(
                name: "SellerPrice",
                table: "Trades",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "StartPrice",
                table: "Trades",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "SkinId",
                table: "SkinPrice",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SkinPrice_Skins_SkinId",
                table: "SkinPrice",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Skins_SkinId",
                table: "Trades",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkinPrice_Skins_SkinId",
                table: "SkinPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Skins_SkinId",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "BuyerPrice",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Float",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "SellerPrice",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "StartPrice",
                table: "Trades");

            migrationBuilder.RenameColumn(
                name: "SkinId",
                table: "Trades",
                newName: "TradeItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_SkinId",
                table: "Trades",
                newName: "IX_Trades_TradeItemId");

            migrationBuilder.AlterColumn<int>(
                name: "SkinId",
                table: "SkinPrice",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Float = table.Column<float>(type: "real", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    SkinId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Skins_SkinId",
                        column: x => x.SkinId,
                        principalTable: "Skins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_SkinId",
                table: "Items",
                column: "SkinId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkinPrice_Skins_SkinId",
                table: "SkinPrice",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Items_TradeItemId",
                table: "Trades",
                column: "TradeItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
