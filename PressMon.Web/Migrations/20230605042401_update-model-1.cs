using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Web.Migrations
{
    public partial class updatemodel1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoacationName",
                table: "LiveDatas",
                newName: "LocationName");

            migrationBuilder.RenameColumn(
                name: "LoacationName",
                table: "Historicals",
                newName: "LocationName");

            migrationBuilder.RenameColumn(
                name: "LoacationName",
                table: "Alarms",
                newName: "LocationName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationName",
                table: "LiveDatas",
                newName: "LoacationName");

            migrationBuilder.RenameColumn(
                name: "LocationName",
                table: "Historicals",
                newName: "LoacationName");

            migrationBuilder.RenameColumn(
                name: "LocationName",
                table: "Alarms",
                newName: "LoacationName");
        }
    }
}
