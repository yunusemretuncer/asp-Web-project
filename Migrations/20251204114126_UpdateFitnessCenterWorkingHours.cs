using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspWebProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFitnessCenterWorkingHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "FitnessCenters");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CloseTime",
                table: "FitnessCenters",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "OpenTime",
                table: "FitnessCenters",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "FitnessCenters");

            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "FitnessCenters");

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "FitnessCenters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
