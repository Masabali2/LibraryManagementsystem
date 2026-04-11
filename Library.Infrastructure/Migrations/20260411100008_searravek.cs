using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class searravek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoysOccupied",
                table: "SeatAvailabilities");

            migrationBuilder.DropColumn(
                name: "BoysTotal",
                table: "SeatAvailabilities");

            migrationBuilder.RenameColumn(
                name: "GirlsTotal",
                table: "SeatAvailabilities",
                newName: "TotalChairs");

            migrationBuilder.RenameColumn(
                name: "GirlsOccupied",
                table: "SeatAvailabilities",
                newName: "PersonsOccupied");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalChairs",
                table: "SeatAvailabilities",
                newName: "GirlsTotal");

            migrationBuilder.RenameColumn(
                name: "PersonsOccupied",
                table: "SeatAvailabilities",
                newName: "GirlsOccupied");

            migrationBuilder.AddColumn<int>(
                name: "BoysOccupied",
                table: "SeatAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BoysTotal",
                table: "SeatAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
