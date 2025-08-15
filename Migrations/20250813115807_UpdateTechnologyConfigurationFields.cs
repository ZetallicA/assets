using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTechnologyConfigurationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Antivirus",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "BackupSolution",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "IPv6Address",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "OSVersion",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "OperatingSystem",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "RemoteAccessID",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "RemoteAccessTool",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "VLAN",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "Workgroup",
                table: "TechnologyConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "License1",
                table: "TechnologyConfigurations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License2",
                table: "TechnologyConfigurations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License3",
                table: "TechnologyConfigurations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License4",
                table: "TechnologyConfigurations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License5",
                table: "TechnologyConfigurations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "License1",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "License2",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "License3",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "License4",
                table: "TechnologyConfigurations");

            migrationBuilder.DropColumn(
                name: "License5",
                table: "TechnologyConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "Antivirus",
                table: "TechnologyConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackupSolution",
                table: "TechnologyConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "TechnologyConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPv6Address",
                table: "TechnologyConfigurations",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OSVersion",
                table: "TechnologyConfigurations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatingSystem",
                table: "TechnologyConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemoteAccessID",
                table: "TechnologyConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemoteAccessTool",
                table: "TechnologyConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VLAN",
                table: "TechnologyConfigurations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Workgroup",
                table: "TechnologyConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
