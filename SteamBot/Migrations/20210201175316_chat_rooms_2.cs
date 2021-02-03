using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamBot.Migrations
{
	public partial class chat_rooms_2 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				"AllMembersInside",
				"ChatRooms",
				"boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<DateTime>(
				"LastMemberChange",
				"ChatRooms",
				"timestamp without time zone",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				"AllMembersInside",
				"ChatRooms");

			migrationBuilder.DropColumn(
				"LastMemberChange",
				"ChatRooms");
		}
	}
}