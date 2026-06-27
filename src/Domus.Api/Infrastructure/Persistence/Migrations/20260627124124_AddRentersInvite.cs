using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domus.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRentersInvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "renters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerTenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    InviteToken = table.Column<string>(type: "text", nullable: true),
                    InviteExpires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_renters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_renters_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_renters_InviteToken",
                table: "renters",
                column: "InviteToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_renters_OwnerTenantId",
                table: "renters",
                column: "OwnerTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_renters_UserId",
                table: "renters",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "renters");
        }
    }
}
