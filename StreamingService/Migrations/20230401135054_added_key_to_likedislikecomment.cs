using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingService.Migrations
{
    /// <inheritdoc />
    public partial class addedkeytolikedislikecomment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_LikedDislikedCommentVotes",
                table: "LikedDislikedCommentVotes",
                columns: new[] { "UserId", "CommentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LikedDislikedCommentVotes",
                table: "LikedDislikedCommentVotes");
        }
    }
}
