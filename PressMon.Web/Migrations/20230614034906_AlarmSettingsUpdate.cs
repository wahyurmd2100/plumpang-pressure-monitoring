using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Web.Migrations
{
    public partial class AlarmSettingsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlarmSettings",
                columns: table => new
                {
                    AlarmSettingID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Info = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmSettings", x => x.AlarmSettingID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlarmSettings");
        }
    }
}
