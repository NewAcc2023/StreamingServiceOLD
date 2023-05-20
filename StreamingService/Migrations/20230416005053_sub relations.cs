using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingService.Migrations
{
    /// <inheritdoc />
    public partial class subrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_FollowerId",
                table: "Subscriptions",
                column: "FollowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_CreatorId",
                table: "Subscriptions",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_FollowerId",
                table: "Subscriptions",
                column: "FollowerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_CreatorId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_FollowerId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_FollowerId",
                table: "Subscriptions");
        }
    }
}
