using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryBackEnd.Migrations
{
    public partial class create5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItem_Category_CategoryId",
                table: "LibraryItem");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "LibraryItem",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItem_Category_CategoryId",
                table: "LibraryItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItem_Category_CategoryId",
                table: "LibraryItem");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "LibraryItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItem_Category_CategoryId",
                table: "LibraryItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
