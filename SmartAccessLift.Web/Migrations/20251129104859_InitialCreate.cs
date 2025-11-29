using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAccessLift.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FloorNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_User_Role", "[Role] IN ('Resident', 'Admin')");
                });

            migrationBuilder.CreateTable(
                name: "FloorPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FloorId = table.Column<int>(type: "int", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    GrantedBy = table.Column<int>(type: "int", nullable: true),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloorPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FloorPermissions_Floors_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FloorPermissions_Users_GrantedBy",
                        column: x => x.GrantedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FloorPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitorAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    VisitorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QRCodeImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UseCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorAccesses", x => x.Id);
                    table.CheckConstraint("CK_VisitorAccess_EndAfterStart", "[EndTime] > [StartTime]");
                    table.CheckConstraint("CK_VisitorAccess_Status", "[Status] IN ('Pending', 'Active', 'Expired')");
                    table.ForeignKey(
                        name: "FK_VisitorAccesses_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    VisitorAccessId = table.Column<int>(type: "int", nullable: true),
                    FloorId = table.Column<int>(type: "int", nullable: false),
                    AccessMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLogs", x => x.Id);
                    table.CheckConstraint("CK_AccessLog_AccessMethod", "[AccessMethod] IN ('NFC', 'Fingerprint', 'QR', 'AdminOverride')");
                    table.CheckConstraint("CK_AccessLog_Outcome", "[Outcome] IN ('Successful', 'Denied')");
                    table.CheckConstraint("CK_AccessLog_UserOrVisitor", "([UserId] IS NOT NULL) OR ([VisitorAccessId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_AccessLogs_Floors_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccessLogs_VisitorAccesses_VisitorAccessId",
                        column: x => x.VisitorAccessId,
                        principalTable: "VisitorAccesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisitorAccessFloors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitorAccessId = table.Column<int>(type: "int", nullable: false),
                    FloorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorAccessFloors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitorAccessFloors_Floors_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitorAccessFloors_VisitorAccesses_VisitorAccessId",
                        column: x => x.VisitorAccessId,
                        principalTable: "VisitorAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_FloorId",
                table: "AccessLogs",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_FloorId_Timestamp",
                table: "AccessLogs",
                columns: new[] { "FloorId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_Outcome",
                table: "AccessLogs",
                column: "Outcome");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_Timestamp",
                table: "AccessLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_UserId",
                table: "AccessLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_UserId_Timestamp",
                table: "AccessLogs",
                columns: new[] { "UserId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_VisitorAccessId",
                table: "AccessLogs",
                column: "VisitorAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPermissions_FloorId",
                table: "FloorPermissions",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPermissions_GrantedBy",
                table: "FloorPermissions",
                column: "GrantedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPermissions_IsAllowed",
                table: "FloorPermissions",
                column: "IsAllowed");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPermissions_UserId",
                table: "FloorPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPermissions_UserId_FloorId",
                table: "FloorPermissions",
                columns: new[] { "UserId", "FloorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Floors_FloorNumber",
                table: "Floors",
                column: "FloorNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAccesses_CreatedAt",
                table: "VisitorAccesses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAccesses_CreatedByUserId",
                table: "VisitorAccesses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAccesses_QRCode",
                table: "VisitorAccesses",
                column: "QRCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAccesses_StartTime_EndTime",
                table: "VisitorAccesses",
                columns: new[] { "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAccesses_Status",
                table: "VisitorAccesses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAccessFloors_FloorId",
                table: "VisitorAccessFloors",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAccessFloors_VisitorAccessId",
                table: "VisitorAccessFloors",
                column: "VisitorAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAccessFloors_VisitorAccessId_FloorId",
                table: "VisitorAccessFloors",
                columns: new[] { "VisitorAccessId", "FloorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLogs");

            migrationBuilder.DropTable(
                name: "FloorPermissions");

            migrationBuilder.DropTable(
                name: "VisitorAccessFloors");

            migrationBuilder.DropTable(
                name: "Floors");

            migrationBuilder.DropTable(
                name: "VisitorAccesses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
