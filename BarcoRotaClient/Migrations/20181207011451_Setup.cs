using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BarcoRota.Client.Migrations
{
    public partial class Setup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BarcoMembers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    NickName = table.Column<string>(maxLength: 15, nullable: false),
                    RotaStatus = table.Column<int>(nullable: false, defaultValue: 0),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcoMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkPackages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManagerId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkPackages_BarcoMembers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "BarcoMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BarcoJobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    JobCapacity = table.Column<int>(nullable: false),
                    JobType = table.Column<int>(nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    WorkPackageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcoJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarcoJobs_WorkPackages_WorkPackageId",
                        column: x => x.WorkPackageId,
                        principalTable: "WorkPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BarcoShifts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BarcoJobId = table.Column<int>(nullable: false),
                    BarcoMemberId = table.Column<int>(nullable: false),
                    ShiftStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcoShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarcoShifts_BarcoJobs_BarcoJobId",
                        column: x => x.BarcoJobId,
                        principalTable: "BarcoJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BarcoShifts_BarcoMembers_BarcoMemberId",
                        column: x => x.BarcoMemberId,
                        principalTable: "BarcoMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarcoJobs_WorkPackageId",
                table: "BarcoJobs",
                column: "WorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_BarcoShifts_BarcoJobId",
                table: "BarcoShifts",
                column: "BarcoJobId");

            migrationBuilder.CreateIndex(
                name: "IX_BarcoShifts_BarcoMemberId",
                table: "BarcoShifts",
                column: "BarcoMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackages_ManagerId",
                table: "WorkPackages",
                column: "ManagerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcoShifts");

            migrationBuilder.DropTable(
                name: "BarcoJobs");

            migrationBuilder.DropTable(
                name: "WorkPackages");

            migrationBuilder.DropTable(
                name: "BarcoMembers");
        }
    }
}
