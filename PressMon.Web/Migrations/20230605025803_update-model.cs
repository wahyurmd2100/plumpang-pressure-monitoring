using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Web.Migrations
{
    public partial class updatemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alarms",
                columns: table => new
                {
                    AlarmID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlarmStatus = table.Column<string>(type: "TEXT", nullable: true),
                    LoacationName = table.Column<string>(type: "TEXT", nullable: true),
                    Pressure = table.Column<double>(type: "REAL", nullable: false),
                    TimeStamp = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarms", x => x.AlarmID);
                });

            migrationBuilder.CreateTable(
                name: "Historicals",
                columns: table => new
                {
                    HistoricalID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoacationName = table.Column<string>(type: "TEXT", nullable: true),
                    Pressure = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeStamp = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historicals", x => x.HistoricalID);
                });

            migrationBuilder.CreateTable(
                name: "LiveDatas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoacationName = table.Column<string>(type: "TEXT", nullable: true),
                    Pressure = table.Column<double>(type: "REAL", nullable: false),
                    TimeStamp = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveDatas", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alarms");

            migrationBuilder.DropTable(
                name: "Historicals");

            migrationBuilder.DropTable(
                name: "LiveDatas");
        }
    }
}
