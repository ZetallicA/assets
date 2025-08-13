using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntraUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserPrincipalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GivenName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OfficeLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmployeeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastSignInDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSyncDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntraUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Supervisor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FloorPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    FloorNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FloorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloorPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FloorPlans_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Desks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FloorPlanId = table.Column<int>(type: "int", nullable: false),
                    DeskNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeskName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    XCoordinate = table.Column<int>(type: "int", nullable: false),
                    YCoordinate = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Desks_FloorPlans_FloorPlanId",
                        column: x => x.FloorPlanId,
                        principalTable: "FloorPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OATH_Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Serial_Number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssetCategoryId = table.Column<int>(type: "int", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Computer_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CostCentre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Assigned_User_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Assigned_User_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Assigned_User_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssignedPersonId = table.Column<int>(type: "int", nullable: true),
                    AssignedEntraUserId = table.Column<int>(type: "int", nullable: true),
                    AssignedEntraObjectId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Phone_Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStatusId = table.Column<int>(type: "int", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Facility = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OS_Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IP_Address = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsCheckedOut = table.Column<bool>(type: "bit", nullable: false),
                    CheckedOutByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CheckedOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCheckInDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentLocationId = table.Column<int>(type: "int", nullable: true),
                    CurrentFloorPlanId = table.Column<int>(type: "int", nullable: true),
                    CurrentDeskId = table.Column<int>(type: "int", nullable: true),
                    Current_Location_Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CurrentBookValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DepreciationRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastDepreciationCalculation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipment_AssetCategories_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Equipment_AssetStatuses_CurrentStatusId",
                        column: x => x.CurrentStatusId,
                        principalTable: "AssetStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Equipment_Desks_CurrentDeskId",
                        column: x => x.CurrentDeskId,
                        principalTable: "Desks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Equipment_EntraUsers_AssignedEntraUserId",
                        column: x => x.AssignedEntraUserId,
                        principalTable: "EntraUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Equipment_FloorPlans_CurrentFloorPlanId",
                        column: x => x.CurrentFloorPlanId,
                        principalTable: "FloorPlans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Equipment_Locations_CurrentLocationId",
                        column: x => x.CurrentLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Equipment_People_AssignedPersonId",
                        column: x => x.AssignedPersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssetAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PerformedByEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreviousStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreviousLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NewLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreviousAssignee = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NewAssignee = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetAuditLogs_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetAuditLogs_EquipmentId",
                table: "AssetAuditLogs",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_Name",
                table: "AssetCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetStatuses_Name",
                table: "AssetStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Desks_FloorPlanId",
                table: "Desks",
                column: "FloorPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_EntraUsers_ObjectId",
                table: "EntraUsers",
                column: "ObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntraUsers_UserPrincipalName",
                table: "EntraUsers",
                column: "UserPrincipalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_AssetCategoryId",
                table: "Equipment",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_AssignedEntraUserId",
                table: "Equipment",
                column: "AssignedEntraUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_AssignedPersonId",
                table: "Equipment",
                column: "AssignedPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_CurrentDeskId",
                table: "Equipment",
                column: "CurrentDeskId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_CurrentFloorPlanId",
                table: "Equipment",
                column: "CurrentFloorPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_CurrentLocationId",
                table: "Equipment",
                column: "CurrentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_CurrentStatusId",
                table: "Equipment",
                column: "CurrentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_OATH_Tag",
                table: "Equipment",
                column: "OATH_Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlans_LocationId",
                table: "FloorPlans",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetAuditLogs");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "AssetCategories");

            migrationBuilder.DropTable(
                name: "AssetStatuses");

            migrationBuilder.DropTable(
                name: "Desks");

            migrationBuilder.DropTable(
                name: "EntraUsers");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "FloorPlans");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
