using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WomenActivity.Migrations
{
    public partial class AddIdToUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_UserProfiles_UserProfileId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "BooksToRead");

            migrationBuilder.RenameIndex(
                name: "IX_Books_UserProfileId",
                table: "BooksToRead",
                newName: "IX_BooksToRead_UserProfileId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BooksToRead",
                table: "BooksToRead",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BooksToRead_UserProfiles_UserProfileId",
                table: "BooksToRead",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Users_UserId",
                table: "UserProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BooksToRead_UserProfiles_UserProfileId",
                table: "BooksToRead");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Users_UserId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BooksToRead",
                table: "BooksToRead");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "BooksToRead",
                newName: "Books");

            migrationBuilder.RenameIndex(
                name: "IX_BooksToRead_UserProfileId",
                table: "Books",
                newName: "IX_Books_UserProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_UserProfiles_UserProfileId",
                table: "Books",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
