using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspWebProject.Migrations
{
    /// <inheritdoc />
    public partial class FixTrainerIdForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrainerId1",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TrainerId1",
                table: "Appointments",
                column: "TrainerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Trainers_TrainerId1",
                table: "Appointments",
                column: "TrainerId1",
                principalTable: "Trainers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Trainers_TrainerId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_TrainerId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "TrainerId1",
                table: "Appointments");
        }
    }
}
