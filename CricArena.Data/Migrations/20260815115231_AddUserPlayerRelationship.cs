using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CricArena.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPlayerRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Players",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Players
                SET UserId = Users.Id
                FROM Players
                INNER JOIN Users ON Players.Email = Users.Email;

                IF EXISTS (SELECT 1 FROM Players WHERE UserId IS NULL)
                BEGIN
                    THROW 50000, 'Every existing Player must have a User with the same email before applying the User-Player relationship migration.', 1;
                END;

                IF EXISTS (SELECT UserId FROM Players GROUP BY UserId HAVING COUNT(*) > 1)
                BEGIN
                    THROW 50001, 'Multiple Players cannot be linked to the same User. Resolve duplicate Player emails before applying the User-Player relationship migration.', 1;
                END;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Players",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Users_UserId",
                table: "Players",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Users_UserId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_UserId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Players");
        }
    }
}
