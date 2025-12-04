using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspWebProject.Migrations
{
    /// <inheritdoc />
    public partial class fix_appointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Appointments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "FitnessCenterId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_FitnessCenterId",
                table: "Appointments",
                column: "FitnessCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_FitnessCenters_FitnessCenterId",
                table: "Appointments",
                column: "FitnessCenterId",
                principalTable: "FitnessCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_FitnessCenters_FitnessCenterId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_FitnessCenterId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "FitnessCenterId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Appointments",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
