using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domus.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlignRenterAndUnitConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_renters_units_UnitId",
                table: "renters");

            migrationBuilder.AlterColumn<string>(
                name: "UnitNumber",
                table: "units",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "InviteToken",
                table: "renters",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_renters_units_UnitId",
                table: "renters",
                column: "UnitId",
                principalTable: "units",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_renters_units_UnitId",
                table: "renters");

            migrationBuilder.AlterColumn<string>(
                name: "UnitNumber",
                table: "units",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "InviteToken",
                table: "renters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_renters_units_UnitId",
                table: "renters",
                column: "UnitId",
                principalTable: "units",
                principalColumn: "Id");
        }
    }
}
