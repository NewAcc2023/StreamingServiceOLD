using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingService.Migrations
{
    /// <inheritdoc />
    public partial class MigrationaddedrelationoneUsermanyVideos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Videos_CreatorId",
                table: "Videos",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Users_CreatorId",
                table: "Videos",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Users_CreatorId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_CreatorId",
                table: "Videos");
        }
    }
}
