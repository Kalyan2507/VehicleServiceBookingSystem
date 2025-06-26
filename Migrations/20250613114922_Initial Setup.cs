using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBook.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Registration",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Registra__1788CC4C88AA8157", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceType",
                columns: table => new
                {
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceT__8ADFAA6C2546C593", x => x.ServiceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCenter",
                columns: table => new
                {
                    ServiceCenterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ServiceCenterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceCenterLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceCenterContact = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceC__71B62BE31C4F7EA0", x => x.ServiceCenterId);
                    table.ForeignKey(
                        name: "FK__ServiceCe__UserI__3A81B327",
                        column: x => x.UserId,
                        principalTable: "Registration",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Make = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vehicle__476B5492C88E34FF", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicle_Registration",
                        column: x => x.UserId,
                        principalTable: "Registration",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Mechanic",
                columns: table => new
                {
                    Mechanicid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCenterId = table.Column<int>(type: "int", nullable: true),
                    MechanicName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Expertise = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mechanic__6B0509C9F62A2D41", x => x.Mechanicid);
                    table.ForeignKey(
                        name: "FK_Mechanic_ServiceCenter",
                        column: x => x.ServiceCenterId,
                        principalTable: "ServiceCenter",
                        principalColumn: "ServiceCenterId");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Bookingid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    VehicleId = table.Column<int>(type: "int", nullable: true),
                    ServiceCenterId = table.Column<int>(type: "int", nullable: true),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    MechanicId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    TimeSlot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__73961EC5AADDA612", x => x.Bookingid);
                    table.ForeignKey(
                        name: "FK_Booking_Registration",
                        column: x => x.UserId,
                        principalTable: "Registration",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Booking_ServiceCenter",
                        column: x => x.ServiceCenterId,
                        principalTable: "ServiceCenter",
                        principalColumn: "ServiceCenterId");
                    table.ForeignKey(
                        name: "FK_Booking_ServiceType",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceType",
                        principalColumn: "ServiceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Vehicle1",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleId");
                    table.ForeignKey(
                        name: "Fk_Booking_Mechanic",
                        column: x => x.MechanicId,
                        principalTable: "Mechanic",
                        principalColumn: "Mechanicid");
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: true),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PaymentStatus = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoice__D796AAB53CBEA5AA", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoice_Booking",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Bookingid");
                    table.ForeignKey(
                        name: "FK_Invoice_ServiceType",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceType",
                        principalColumn: "ServiceTypeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_MechanicId",
                table: "Booking",
                column: "MechanicId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_ServiceCenterId",
                table: "Booking",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_ServiceTypeId",
                table: "Booking",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_UserId",
                table: "Booking",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_VehicleId",
                table: "Booking",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BookingId",
                table: "Invoice",
                column: "BookingId",
                unique: true,
                filter: "[BookingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ServiceTypeId",
                table: "Invoice",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Mechanic_ServiceCenterId",
                table: "Mechanic",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "UQ__Registra__A9D10534FDAE5997",
                table: "Registration",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCenter_UserId",
                table: "ServiceCenter",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_UserId",
                table: "Vehicle",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "ServiceType");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Mechanic");

            migrationBuilder.DropTable(
                name: "ServiceCenter");

            migrationBuilder.DropTable(
                name: "Registration");
        }
    }
}
