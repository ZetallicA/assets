using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnologyConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnologyConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    NetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IPv4Address = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IPv6Address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    MACAddress = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    WallPort = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SwitchPort = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SwitchName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VLAN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IMEI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SIMCardNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Vendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ServicePlan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Workgroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OperatingSystem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OSVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Antivirus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RemoteAccessTool = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RemoteAccessID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BackupSolution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConfigurationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnologyConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnologyConfigurations_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnologyConfigurations_EquipmentId",
                table: "TechnologyConfigurations",
                column: "EquipmentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnologyConfigurations");
        }
    }
}
