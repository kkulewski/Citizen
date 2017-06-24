using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Citizen.Data.Migrations
{
    public partial class FixFoodItemInheritance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "FoodItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EnergyRecoverAmount",
                table: "FoodItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FoodItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "FoodItem");

            migrationBuilder.DropColumn(
                name: "EnergyRecoverAmount",
                table: "FoodItem");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FoodItem");
        }
    }
}
