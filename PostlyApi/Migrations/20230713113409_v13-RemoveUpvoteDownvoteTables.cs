using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostlyApi.Migrations
{
    /// <inheritdoc />
    public partial class v13RemoveUpvoteDownvoteTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Downvotes");

            migrationBuilder.DropTable(
                name: "Upvotes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Downvotes",
                columns: table => new
                {
                    DownvotedById = table.Column<long>(type: "bigint", nullable: false),
                    DownvotedPostsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downvotes", x => new { x.DownvotedById, x.DownvotedPostsId });
                    table.ForeignKey(
                        name: "FK_Downvotes_Posts_DownvotedPostsId",
                        column: x => x.DownvotedPostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Downvotes_Users_DownvotedById",
                        column: x => x.DownvotedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Upvotes",
                columns: table => new
                {
                    UpvotedById = table.Column<long>(type: "bigint", nullable: false),
                    UpvotedPostsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Upvotes", x => new { x.UpvotedById, x.UpvotedPostsId });
                    table.ForeignKey(
                        name: "FK_Upvotes_Posts_UpvotedPostsId",
                        column: x => x.UpvotedPostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Upvotes_Users_UpvotedById",
                        column: x => x.UpvotedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Downvotes_DownvotedPostsId",
                table: "Downvotes",
                column: "DownvotedPostsId");

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_UpvotedPostsId",
                table: "Upvotes",
                column: "UpvotedPostsId");
        }
    }
}
