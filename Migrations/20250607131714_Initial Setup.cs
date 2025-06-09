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
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Registra__1788CC4CFFFF47B6", x => x.UserId);
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
                    table.PrimaryKey("PK__ServiceT__8ADFAA6C42E00C64", x => x.ServiceTypeId);
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
                    ServiceCenterContact = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceC__71B62BE37DB8DD79", x => x.ServiceCenterId);
                    table.ForeignKey(
                        name: "FK__ServiceCe__UserI__4E88ABD4",
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
                    table.PrimaryKey("PK__Vehicle__476B5492B47F68A7", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK__Vehicle__UserId__5629CD9C",
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
                    table.PrimaryKey("PK__Mechanic__6B0509C952C35A70", x => x.Mechanicid);
                    table.ForeignKey(
                        name: "FK__Mechanic__Servic__5165187F",
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
                    RegistrationUserId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    VehicleId = table.Column<int>(type: "int", nullable: true),
                    ServiceCenterId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeSlot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true, defaultValue: "Pending"),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__73961EC5F67A7B8A", x => x.Bookingid);
                    table.ForeignKey(
                        name: "FK_Booking_Registration_RegistrationUserId",
                        column: x => x.RegistrationUserId,
                        principalTable: "Registration",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_ServiceType_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceType",
                        principalColumn: "ServiceTypeId");
                    table.ForeignKey(
                        name: "FK__Booking__UserId__59063A47",
                        column: x => x.UserId,
                        principalTable: "ServiceCenter",
                        principalColumn: "ServiceCenterId");
                    table.ForeignKey(
                        name: "FK__Booking__Vehicle__59FA5E80",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: true),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<double>(type: "float", nullable: true),
                    PaymentStatus = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoice__D796AAB53F7BFBC9", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK__Invoice__Booking__6FE99F9F",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Bookingid");
                    table.ForeignKey(
                        name: "FK__Invoice__Service__70DDC3D8",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceType",
                        principalColumn: "ServiceTypeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_RegistrationUserId",
                table: "Booking",
                column: "RegistrationUserId");

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
                name: "IX_Invoice_ServiceTypeId",
                table: "Invoice",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "UQ__Invoice__73951AEC734F6989",
                table: "Invoice",
                column: "BookingId",
                unique: true,
                filter: "[BookingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Mechanic_ServiceCenterId",
                table: "Mechanic",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "UQ__Registra__5C7E359EB11CB881",
                table: "Registration",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Registra__A9D105349468CAF8",
                table: "Registration",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCenter_UserId",
                table: "ServiceCenter",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__ServiceC__F2124AAD32FA906C",
                table: "ServiceCenter",
                column: "ServiceCenterContact",
                unique: true,
                filter: "[ServiceCenterContact] IS NOT NULL");

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
                name: "Mechanic");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "ServiceType");

            migrationBuilder.DropTable(
                name: "ServiceCenter");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Registration");
        }
    }
}
