using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkProductOptionToDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductOptions");

            migrationBuilder.DropColumn(
                name: "InputType",
                table: "ProductOptions");

            migrationBuilder.AddColumn<int>(
                name: "ProductOptionDefinitionId",
                table: "ProductOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductOptions_ProductOptionDefinitionId",
                table: "ProductOptions",
                column: "ProductOptionDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptions_ProductOptionDefinitions_ProductOptionDefinitionId",
                table: "ProductOptions",
                column: "ProductOptionDefinitionId",
                principalTable: "ProductOptionDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptions_ProductOptionDefinitions_ProductOptionDefinitionId",
                table: "ProductOptions");

            migrationBuilder.DropIndex(
                name: "IX_ProductOptions_ProductOptionDefinitionId",
                table: "ProductOptions");

            migrationBuilder.DropColumn(
                name: "ProductOptionDefinitionId",
                table: "ProductOptions");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductOptions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "InputType",
                table: "ProductOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}