using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pawpals.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMemberDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Members_FollowerId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Members_FollowingId",
                table: "Connections");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Members_FollowerId",
                table: "Connections",
                column: "FollowerId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Members_FollowingId",
                table: "Connections",
                column: "FollowingId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Members_FollowerId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Members_FollowingId",
                table: "Connections");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Members_FollowerId",
                table: "Connections",
                column: "FollowerId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Members_FollowingId",
                table: "Connections",
                column: "FollowingId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
