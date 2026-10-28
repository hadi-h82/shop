using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMobileOtpAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OtpChallenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    FailedAttemptCount = table.Column<int>(type: "int", nullable: false),
                    MaxAttempts = table.Column<int>(type: "int", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvalidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpChallenges", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "PhoneNumberIndex",
                table: "AspNetUsers",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_CreatedByIp_CreatedAt",
                table: "OtpChallenges",
                columns: new[] { "CreatedByIp", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_ExpiresAt",
                table: "OtpChallenges",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_PhoneNumber_Purpose_CreatedAt",
                table: "OtpChallenges",
                columns: new[] { "PhoneNumber", "Purpose", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpChallenges");

            migrationBuilder.DropIndex(
                name: "PhoneNumberIndex",
                table: "AspNetUsers");
        }
    }
}
