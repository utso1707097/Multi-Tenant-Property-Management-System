using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domus.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExpandRenters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "renters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateOnly>(
                name: "LeaseEndDate",
                table: "renters",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "MoveInDate",
                table: "renters",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                table: "renters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "renters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_renters_UnitId",
                table: "renters",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_renters_units_UnitId",
                table: "renters",
                column: "UnitId",
                principalTable: "units",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_renters_units_UnitId",
                table: "renters");

            migrationBuilder.DropIndex(
                name: "IX_renters_UnitId",
                table: "renters");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "renters");

            migrationBuilder.DropColumn(
                name: "LeaseEndDate",
                table: "renters");

            migrationBuilder.DropColumn(
                name: "MoveInDate",
                table: "renters");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "renters");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "renters");
        }
    }
}
