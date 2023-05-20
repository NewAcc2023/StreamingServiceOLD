using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingService.Migrations
{
    /// <inheritdoc />
    public partial class x4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LikedDislikedVideoVotes_VideoId",
                table: "LikedDislikedVideoVotes",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_LikedDislikedCommentVotes_CommentId",
                table: "LikedDislikedCommentVotes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_VideoId",
                table: "Comments",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Videos_VideoId",
                table: "Comments",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikedDislikedCommentVotes_Comments_CommentId",
                table: "LikedDislikedCommentVotes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikedDislikedCommentVotes_Users_UserId",
                table: "LikedDislikedCommentVotes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikedDislikedVideoVotes_Users_UserId",
                table: "LikedDislikedVideoVotes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikedDislikedVideoVotes_Videos_VideoId",
                table: "LikedDislikedVideoVotes",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
