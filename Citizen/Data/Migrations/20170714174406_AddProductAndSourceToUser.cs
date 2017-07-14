using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Citizen.Data.Migrations
{
    public partial class AddProductAndSourceToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Companies",
                newName: "MaxEmployments");

            migrationBuilder.AddColumn<int>(
                name: "Product",
                table: "Companies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Product",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "MaxEmployments",
                table: "Companies",
                newName: "Type");
        }
    }
}
