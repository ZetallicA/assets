using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddSimplifiedEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServicePlan",
                table: "TechnologyConfigurations");

            migrationBuilder.CreateTable(
                name: "SimplifiedEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OATH_Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Serial_Number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Asset_Tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Service_Tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Computer_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Assigned_User_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Assigned_User_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Desk = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Net_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IP_Address = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    MAC_Address = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    Wall_Port = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Switch_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Switch_Port = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Phone_Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IMEI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SIM_Card_Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OS_Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    License1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    License2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    License3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    License4 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    License5 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Purchase_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Purchase_Order_Number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Vendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Purchase_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Warranty_Start_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Warranty_End_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Configuration_Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsCheckedOut = table.Column<bool>(type: "bit", nullable: false),
                    CheckedOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedOutBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimplifiedEquipment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimplifiedEquipment_OATH_Tag",
                table: "SimplifiedEquipment",
                column: "OATH_Tag",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimplifiedEquipment");

            migrationBuilder.AddColumn<string>(
                name: "ServicePlan",
                table: "TechnologyConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
