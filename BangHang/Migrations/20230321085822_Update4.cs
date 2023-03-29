using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BangHang.Migrations
{
    public partial class Update4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "Sold",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "CategoriesPro",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "Sold",
               table: "Products");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "CategoriesPro");
        }
    }
}
