using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CricArena.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationModuleAndUniquePlayerPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RepondedOn",
                table: "Invitations",
                newName: "RespondedOn");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Players",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Invitations",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                "UPDATE Invitations SET Status = CASE WHEN IsAccepted = 1 THEN 2 ELSE 1 END;");

            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "Invitations");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PhoneNumber",
                table: "Players",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_PhoneNumber",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "RespondedOn",
                table: "Invitations",
                newName: "RepondedOn");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "Invitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                "UPDATE Invitations SET IsAccepted = CASE WHEN Status = 2 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END;");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invitations");
        }
    }
}
