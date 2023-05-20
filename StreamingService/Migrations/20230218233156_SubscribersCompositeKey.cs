using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingService.Migrations
{
    /// <inheritdoc />
    public partial class SubscribersCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                columns: new[] { "CreatorId", "FollowerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");
        }
    }
}
