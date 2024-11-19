using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WomenActivity.Migrations
{
    public partial class UpdateDbUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BooksToRead_UserProfiles_UserProfileId",
                table: "BooksToRead");

            migrationBuilder.DropForeignKey(
                name: "FK_BooksToRead_Users_UserId",
                table: "BooksToRead");

            migrationBuilder.DropForeignKey(
                name: "FK_Goals_UserProfiles_UserProfileId",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Users_UserId",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Memories_UserProfiles_UserProfileId",
                table: "Memories");

            migrationBuilder.DropForeignKey(
                name: "FK_Memories_Users_UserId",
                table: "Memories");

            migrationBuilder.DropIndex(
                name: "IX_Memories_UserId",
                table: "Memories");

            migrationBuilder.DropIndex(
                name: "IX_Goals_UserId",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_BooksToRead_UserId",
                table: "BooksToRead");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Memories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BooksToRead");

            migrationBuilder.AlterColumn<int>(
                name: "UserProfileId",
                table: "Memories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserProfileId",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserProfileId",
                table: "BooksToRead",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BooksToRead_UserProfiles_UserProfileId",
                table: "BooksToRead",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_UserProfiles_UserProfileId",
                table: "Goals",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Memories_UserProfiles_UserProfileId",
                table: "Memories",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BooksToRead_UserProfiles_UserProfileId",
                table: "BooksToRead");

            migrationBuilder.DropForeignKey(
                name: "FK_Goals_UserProfiles_UserProfileId",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Memories_UserProfiles_UserProfileId",
                table: "Memories");

            migrationBuilder.AlterColumn<int>(
                name: "UserProfileId",
                table: "Memories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Memories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UserProfileId",
                table: "Goals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UserProfileId",
                table: "BooksToRead",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BooksToRead",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Memories_UserId",
                table: "Memories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserId",
                table: "Goals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BooksToRead_UserId",
                table: "BooksToRead",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BooksToRead_UserProfiles_UserProfileId",
                table: "BooksToRead",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BooksToRead_Users_UserId",
                table: "BooksToRead",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_UserProfiles_UserProfileId",
                table: "Goals",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Users_UserId",
                table: "Goals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Memories_UserProfiles_UserProfileId",
                table: "Memories",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Memories_Users_UserId",
                table: "Memories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
