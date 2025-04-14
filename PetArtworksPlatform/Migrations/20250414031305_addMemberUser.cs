using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetArtworksPlatform.Migrations
{
    /// <inheritdoc />
    public partial class addMemberUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_UserId",
                table: "Members");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberUserId",
                table: "Members",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberUserId",
                table: "Members",
                column: "MemberUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_MemberUserId",
                table: "Members",
                column: "MemberUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_MemberUserId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_MemberUserId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MemberUserId",
                table: "Members");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Members",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_UserId",
                table: "Members",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
