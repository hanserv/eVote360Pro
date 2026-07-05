using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360Pro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixPoliticalLeaderRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoliticalParties_Users_UserId",
                table: "PoliticalParties");

            migrationBuilder.DropIndex(
                name: "IX_PoliticalParties_UserId",
                table: "PoliticalParties");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PoliticalParties");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PoliticalPartyId",
                table: "Users",
                column: "PoliticalPartyId",
                unique: true,
                filter: "[PoliticalPartyId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PoliticalParties_PoliticalPartyId",
                table: "Users",
                column: "PoliticalPartyId",
                principalTable: "PoliticalParties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_PoliticalParties_PoliticalPartyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PoliticalPartyId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PoliticalParties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PoliticalParties_UserId",
                table: "PoliticalParties",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PoliticalParties_Users_UserId",
                table: "PoliticalParties",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
