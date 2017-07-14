using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Citizen.Data.Migrations
{
    public partial class AddEmploymentToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_AspNetUsers_EmployeeId1",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_Employments_EmployeeId1",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "Employments");

            migrationBuilder.AddColumn<int>(
                name: "EmploymentId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmploymentId",
                table: "AspNetUsers",
                column: "EmploymentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Employments_EmploymentId",
                table: "AspNetUsers",
                column: "EmploymentId",
                principalTable: "Employments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Employments_EmploymentId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmploymentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmploymentId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId1",
                table: "Employments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_EmployeeId1",
                table: "Employments",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_AspNetUsers_EmployeeId1",
                table: "Employments",
                column: "EmployeeId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
