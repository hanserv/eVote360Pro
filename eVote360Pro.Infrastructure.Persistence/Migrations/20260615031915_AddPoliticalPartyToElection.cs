using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360Pro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPoliticalPartyToElection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PoliticalPartyId",
                table: "Votes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PoliticalPartyId",
                table: "Votes",
                column: "PoliticalPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_PoliticalParties_PoliticalPartyId",
                table: "Votes",
                column: "PoliticalPartyId",
                principalTable: "PoliticalParties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_PoliticalParties_PoliticalPartyId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_PoliticalPartyId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "PoliticalPartyId",
                table: "Votes");
        }
    }
}
