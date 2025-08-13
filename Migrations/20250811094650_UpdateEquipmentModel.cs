using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEquipmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Asset_Tag",
                table: "Equipment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Current_Value",
                table: "Equipment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Equipment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Last_Maintenance_Date",
                table: "Equipment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Equipment",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Purchase_Cost",
                table: "Equipment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Purchase_Date",
                table: "Equipment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Warranty_Expiry",
                table: "Equipment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "EntraUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "AssetAuditLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "AssetAuditLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AssetAuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asset_Tag",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Current_Value",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Last_Maintenance_Date",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Purchase_Cost",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Purchase_Date",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Warranty_Expiry",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "EntraUsers");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "AssetAuditLogs");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "AssetAuditLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AssetAuditLogs");
        }
    }
}
