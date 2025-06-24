using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    Telegram_Id = table.Column<long>(type: "bigint", nullable: false),
                    First_name = table.Column<string>(type: "text", nullable: false),
                    Last_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Periodicity",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    User_Id = table.Column<string>(type: "text", nullable: false),
                    Periodicity_Id = table.Column<int>(type: "integer", nullable: false),
                    StartNotify = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periodicity", x => x.id);
                    table.ForeignKey(
                        name: "FK_Periodicity_Candidates_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Candidates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscribedVacancy",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    User_Id = table.Column<string>(type: "text", nullable: false),
                    Speciality_Id = table.Column<int>(type: "integer", nullable: true),
                    Subspecialty_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribedVacancy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscribedVacancy_Candidates_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Candidates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Periodicity_User_Id",
                table: "Periodicity",
                column: "User_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscribedVacancy_User_Id",
                table: "SubscribedVacancy",
                column: "User_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Periodicity");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "SubscribedVacancy");

            migrationBuilder.DropTable(
                name: "Candidates");
        }
    }
}
