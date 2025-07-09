using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crysta.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionToAppUserAndOtherThings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRole_AppRole_AppRoleId",
                table: "AppUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRole_AppUser_AppUserId",
                table: "AppUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppRole",
                table: "AppRole");

            migrationBuilder.DropIndex(
                name: "IX_AppRole_RoleName",
                table: "AppRole");

            migrationBuilder.RenameTable(
                name: "AppRole",
                newName: "AppRoles");

            migrationBuilder.RenameColumn(
                name: "AppRoleId",
                table: "AppUserRole",
                newName: "AppRole_ID");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "AppUserRole",
                newName: "AppUser_ID");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserRole_AppRoleId",
                table: "AppUserRole",
                newName: "IX_AppUserRole_AppRole_ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AppUser",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AppRoles",
                newName: "ID");

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "AppUser",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppRoles",
                table: "AppRoles",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRole_AppUser_ID",
                table: "AppUserRole",
                column: "AppUser_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRole_AppRoles_AppRole_ID",
                table: "AppUserRole",
                column: "AppRole_ID",
                principalTable: "AppRoles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRole_AppUser_AppUser_ID",
                table: "AppUserRole",
                column: "AppUser_ID",
                principalTable: "AppUser",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRole_AppRoles_AppRole_ID",
                table: "AppUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRole_AppUser_AppUser_ID",
                table: "AppUserRole");

            migrationBuilder.DropIndex(
                name: "IX_AppUserRole_AppUser_ID",
                table: "AppUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppRoles",
                table: "AppRoles");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "AppUser");

            migrationBuilder.RenameTable(
                name: "AppRoles",
                newName: "AppRole");

            migrationBuilder.RenameColumn(
                name: "AppRole_ID",
                table: "AppUserRole",
                newName: "AppRoleId");

            migrationBuilder.RenameColumn(
                name: "AppUser_ID",
                table: "AppUserRole",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserRole_AppRole_ID",
                table: "AppUserRole",
                newName: "IX_AppUserRole_AppRoleId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "AppUser",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "AppRole",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppRole",
                table: "AppRole",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppRole_RoleName",
                table: "AppRole",
                column: "RoleName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRole_AppRole_AppRoleId",
                table: "AppUserRole",
                column: "AppRoleId",
                principalTable: "AppRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRole_AppUser_AppUserId",
                table: "AppUserRole",
                column: "AppUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
