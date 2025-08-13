using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTagToEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Service_Tag",
                table: "Equipment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Service_Tag",
                table: "Equipment");
        }
    }
}
