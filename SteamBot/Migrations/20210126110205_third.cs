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
                name: "FK_Items_Images_ImageId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Items_ItemId",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "IsKnife",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsStatTrak",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MarketPrice",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SkinName",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "WeaponName",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Trades",
                newName: "TradeItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_ItemId",
                table: "Trades",
                newName: "IX_Trades_TradeItemId");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "Items",
                newName: "SkinId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_ImageId",
                table: "Items",
                newName: "IX_Items_SkinId");

            migrationBuilder.CreateTable(
                name: "Skins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponName = table.Column<string>(type: "text", nullable: true),
                    IsKnife = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatTrak = table.Column<bool>(type: "boolean", nullable: false),
                    SkinName = table.Column<string>(type: "text", nullable: true),
                    BattleScarredPrice = table.Column<double>(type: "double precision", nullable: true),
                    FactoryNewPrice = table.Column<double>(type: "double precision", nullable: true),
                    FieldTestedPrice = table.Column<double>(type: "double precision", nullable: true),
                    MinimalWearPrice = table.Column<double>(type: "double precision", nullable: true),
                    WellWornPrice = table.Column<double>(type: "double precision", nullable: true),
                    BattleScarredImageId = table.Column<int>(type: "integer", nullable: true),
                    FactoryNewImageId = table.Column<int>(type: "integer", nullable: true),
                    FieldTestedImageId = table.Column<int>(type: "integer", nullable: true),
                    MinimalWearImageId = table.Column<int>(type: "integer", nullable: true),
                    WellWornImageId = table.Column<int>(type: "integer", nullable: true),
                    CreateTS = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdateTS = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skins_Images_BattleScarredImageId",
                        column: x => x.BattleScarredImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skins_Images_FactoryNewImageId",
                        column: x => x.FactoryNewImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skins_Images_FieldTestedImageId",
                        column: x => x.FieldTestedImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skins_Images_MinimalWearImageId",
                        column: x => x.MinimalWearImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skins_Images_WellWornImageId",
                        column: x => x.WellWornImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skins_BattleScarredImageId",
                table: "Skins",
                column: "BattleScarredImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_FactoryNewImageId",
                table: "Skins",
                column: "FactoryNewImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_FieldTestedImageId",
                table: "Skins",
                column: "FieldTestedImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_MinimalWearImageId",
                table: "Skins",
                column: "MinimalWearImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_WellWornImageId",
                table: "Skins",
                column: "WellWornImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Skins_SkinId",
                table: "Items",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Skins_SkinId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Items_TradeItemId",
                table: "Trades");

            migrationBuilder.DropTable(
                name: "Skins");

            migrationBuilder.RenameColumn(
                name: "TradeItemId",
                table: "Trades",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_TradeItemId",
                table: "Trades",
                newName: "IX_Trades_ItemId");

            migrationBuilder.RenameColumn(
                name: "SkinId",
                table: "Items",
                newName: "ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_SkinId",
                table: "Items",
                newName: "IX_Items_ImageId");

            migrationBuilder.AddColumn<bool>(
                name: "IsKnife",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStatTrak",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "MarketPrice",
                table: "Items",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "SkinName",
                table: "Items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeaponName",
                table: "Items",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Images_ImageId",
                table: "Items",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Items_ItemId",
                table: "Trades",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
