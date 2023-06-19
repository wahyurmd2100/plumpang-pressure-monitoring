using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Web.Migrations
{
    public partial class AlarmSettings_Add_UpdateTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UpdateTimestamp",
                table: "AlarmSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateTimestamp",
                table: "AlarmSettings");
        }
    }
}
