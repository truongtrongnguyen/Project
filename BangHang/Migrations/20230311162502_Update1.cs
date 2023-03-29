using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BangHang.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Posts",
                newName: "DesCriptions");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Posts",
                newName: "Contents");

            migrationBuilder.RenameColumn(
                name: "SeoDesCription",
                table: "Categories",
                newName: "SeoDesCriptions");

            migrationBuilder.RenameColumn(
                name: "Desciption",
                table: "Categories",
                newName: "DesCriptions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DesCriptions",
                table: "Posts",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Contents",
                table: "Posts",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "SeoDesCriptions",
                table: "Categories",
                newName: "SeoDesCription");

            migrationBuilder.RenameColumn(
                name: "DesCriptions",
                table: "Categories",
                newName: "Desciption");
        }
    }
}
