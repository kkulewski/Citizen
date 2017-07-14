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
                name: "EmployeeId",
                table: "Employments");

            migrationBuilder.RenameColumn(
                name: "EmployeeId1",
                table: "Employments",
                newName: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_ApplicationUserId",
                table: "Employments",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_AspNetUsers_ApplicationUserId",
                table: "Employments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_AspNetUsers_ApplicationUserId",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_Employments_ApplicationUserId",
                table: "Employments");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Employments",
                newName: "EmployeeId1");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Employments",
                nullable: false,
                defaultValue: 0);

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
