using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Citizen.Data.Migrations
{
    public partial class AddStorageCitizenAlternateKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStorage_AspNetUsers_ApplicationUserId",
                table: "UserStorage");

            migrationBuilder.DropIndex(
                name: "IX_UserStorage_ApplicationUserId",
                table: "UserStorage");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "UserStorage",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_StorageCitizenId",
                table: "UserStorage",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStorage_AspNetUsers_ApplicationUserId",
                table: "UserStorage",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStorage_AspNetUsers_ApplicationUserId",
                table: "UserStorage");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_StorageCitizenId",
                table: "UserStorage");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "UserStorage",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_UserStorage_ApplicationUserId",
                table: "UserStorage",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStorage_AspNetUsers_ApplicationUserId",
                table: "UserStorage",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
