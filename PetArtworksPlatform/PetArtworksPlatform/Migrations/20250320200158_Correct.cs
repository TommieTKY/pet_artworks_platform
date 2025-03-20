using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetArtworksPlatform.Migrations
{
    /// <inheritdoc />
    public partial class Correct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtworkPet_Pet_PetId",
                table: "ArtworkPet");

            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Member_FollowerId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Member_FollowingId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Member_AspNetUsers_UserId",
                table: "Member");

            migrationBuilder.DropForeignKey(
                name: "FK_PetOwner_Member_OwnerId",
                table: "PetOwner");

            migrationBuilder.DropForeignKey(
                name: "FK_PetOwner_Pet_PetId",
                table: "PetOwner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PetOwner",
                table: "PetOwner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pet",
                table: "Pet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Member",
                table: "Member");

            migrationBuilder.RenameTable(
                name: "PetOwner",
                newName: "PetOwners");

            migrationBuilder.RenameTable(
                name: "Pet",
                newName: "Pets");

            migrationBuilder.RenameTable(
                name: "Member",
                newName: "Members");

            migrationBuilder.RenameIndex(
                name: "IX_PetOwner_PetId",
                table: "PetOwners",
                newName: "IX_PetOwners_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_PetOwner_OwnerId",
                table: "PetOwners",
                newName: "IX_PetOwners_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Member_UserId",
                table: "Members",
                newName: "IX_Members_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PetOwners",
                table: "PetOwners",
                column: "PetOwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pets",
                table: "Pets",
                column: "PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtworkPet_Pets_PetId",
                table: "ArtworkPet",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);

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
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PetOwners_Members_OwnerId",
                table: "PetOwners",
                column: "OwnerId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PetOwners_Pets_PetId",
                table: "PetOwners",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtworkPet_Pets_PetId",
                table: "ArtworkPet");

            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Members_FollowerId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Members_FollowingId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_PetOwners_Members_OwnerId",
                table: "PetOwners");

            migrationBuilder.DropForeignKey(
                name: "FK_PetOwners_Pets_PetId",
                table: "PetOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pets",
                table: "Pets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PetOwners",
                table: "PetOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.RenameTable(
                name: "Pets",
                newName: "Pet");

            migrationBuilder.RenameTable(
                name: "PetOwners",
                newName: "PetOwner");

            migrationBuilder.RenameTable(
                name: "Members",
                newName: "Member");

            migrationBuilder.RenameIndex(
                name: "IX_PetOwners_PetId",
                table: "PetOwner",
                newName: "IX_PetOwner_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_PetOwners_OwnerId",
                table: "PetOwner",
                newName: "IX_PetOwner_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Members_UserId",
                table: "Member",
                newName: "IX_Member_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pet",
                table: "Pet",
                column: "PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PetOwner",
                table: "PetOwner",
                column: "PetOwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Member",
                table: "Member",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtworkPet_Pet_PetId",
                table: "ArtworkPet",
                column: "PetId",
                principalTable: "Pet",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Member_FollowerId",
                table: "Connections",
                column: "FollowerId",
                principalTable: "Member",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Member_FollowingId",
                table: "Connections",
                column: "FollowingId",
                principalTable: "Member",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Member_AspNetUsers_UserId",
                table: "Member",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PetOwner_Member_OwnerId",
                table: "PetOwner",
                column: "OwnerId",
                principalTable: "Member",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PetOwner_Pet_PetId",
                table: "PetOwner",
                column: "PetId",
                principalTable: "Pet",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
