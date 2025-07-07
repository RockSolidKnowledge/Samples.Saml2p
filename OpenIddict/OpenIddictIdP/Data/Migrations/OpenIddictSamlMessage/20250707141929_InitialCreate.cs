using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenIddictIdP.Data.Migrations.OpenIddictSamlMessage
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenIddctSamlMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RequestId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Expiration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddctSamlMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddctSamlMessages_CreationTime",
                table: "OpenIddctSamlMessages",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddctSamlMessages_Expiration",
                table: "OpenIddctSamlMessages",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddctSamlMessages_RequestId",
                table: "OpenIddctSamlMessages",
                column: "RequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenIddctSamlMessages");
        }
    }
}
