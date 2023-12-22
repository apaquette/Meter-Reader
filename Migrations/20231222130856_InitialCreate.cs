using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnergyInsightHub.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MicroControllers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicroControllers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meters_MicroControllers_Id",
                        column: x => x.Id,
                        principalTable: "MicroControllers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MicrocontrollerAssignment",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ControllerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TheMeterId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicrocontrollerAssignment", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerAssignment_Meters_TheMeterId",
                        column: x => x.TheMeterId,
                        principalTable: "Meters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerAssignment_MicroControllers_ControllerId",
                        column: x => x.ControllerId,
                        principalTable: "MicroControllers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reading",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EnergyMeterId = table.Column<int>(type: "INTEGER", nullable: false),
                    MicrocontrollerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reading_Meters_EnergyMeterId",
                        column: x => x.EnergyMeterId,
                        principalTable: "Meters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reading_MicroControllers_MicrocontrollerId",
                        column: x => x.MicrocontrollerId,
                        principalTable: "MicroControllers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerAssignment_ControllerId",
                table: "MicrocontrollerAssignment",
                column: "ControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerAssignment_TheMeterId",
                table: "MicrocontrollerAssignment",
                column: "TheMeterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reading_EnergyMeterId",
                table: "Reading",
                column: "EnergyMeterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reading_MicrocontrollerId",
                table: "Reading",
                column: "MicrocontrollerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MicrocontrollerAssignment");

            migrationBuilder.DropTable(
                name: "Reading");

            migrationBuilder.DropTable(
                name: "Meters");

            migrationBuilder.DropTable(
                name: "MicroControllers");
        }
    }
}
