using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Citizen.Data.Migrations
{
    public partial class NewItemLogic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoodAmount",
                table: "UserStorage");

            migrationBuilder.DropColumn(
                name: "GrainAmount",
                table: "UserStorage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FoodAmount",
                table: "UserStorage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GrainAmount",
                table: "UserStorage",
                nullable: false,
                defaultValue: 0);
        }
    }
}
