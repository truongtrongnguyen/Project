using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BangHang.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_CategoryParentID",
                table: "Categories");

            migrationBuilder.AddColumn<bool>(
                name: "IsHot",
                table: "Posts",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryParentID",
                table: "Categories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_CategoryParentID",
                table: "Categories",
                column: "CategoryParentID",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_CategoryParentID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsHot",
                table: "Posts");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryParentID",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_CategoryParentID",
                table: "Categories",
                column: "CategoryParentID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
