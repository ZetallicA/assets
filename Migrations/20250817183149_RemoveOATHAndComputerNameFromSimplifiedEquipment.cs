using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOATHAndComputerNameFromSimplifiedEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SimplifiedEquipment_OATH_Tag",
                table: "SimplifiedEquipment");

            migrationBuilder.DropColumn(
                name: "Computer_Name",
                table: "SimplifiedEquipment");

            migrationBuilder.DropColumn(
                name: "OATH_Tag",
                table: "SimplifiedEquipment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Computer_Name",
                table: "SimplifiedEquipment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OATH_Tag",
                table: "SimplifiedEquipment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SimplifiedEquipment_OATH_Tag",
                table: "SimplifiedEquipment",
                column: "OATH_Tag",
                unique: true);
        }
    }
}
