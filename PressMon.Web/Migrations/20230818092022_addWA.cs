using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Web.Migrations
{
    public partial class addWA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaAlarmMessageStatuses",
                columns: table => new
                {
                    AlarmId = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<int>(type: "INTEGER", nullable: false),
                    ApiResponse = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaAlarmMessageStatuses", x => x.AlarmId);
                    table.ForeignKey(
                        name: "FK_WaAlarmMessageStatuses_Alarms_AlarmId",
                        column: x => x.AlarmId,
                        principalTable: "Alarms",
                        principalColumn: "AlarmID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaAlarmMessageStatuses");
        }
    }
}
