using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360Pro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeSnapshotsToElection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalCandidates",
                table: "Elections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalParties",
                table: "Elections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalPositions",
                table: "Elections",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCandidates",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "TotalParties",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "TotalPositions",
                table: "Elections");
        }
    }
}
