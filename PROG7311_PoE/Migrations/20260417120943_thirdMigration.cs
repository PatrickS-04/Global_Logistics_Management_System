using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG7311_PoE.Migrations
{
    /// <inheritdoc />
    public partial class thirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "clientRegion",
                table: "Clients",
                newName: "clientPhoneNumber");

            migrationBuilder.AddColumn<int>(
                name: "LogisticsManagerId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LogisticsManager",
                columns: table => new
                {
                    LogisticsManagerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticsManager", x => x.LogisticsManagerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_LogisticsManagerId",
                table: "Contracts",
                column: "LogisticsManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_LogisticsManager_LogisticsManagerId",
                table: "Contracts",
                column: "LogisticsManagerId",
                principalTable: "LogisticsManager",
                principalColumn: "LogisticsManagerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_LogisticsManager_LogisticsManagerId",
                table: "Contracts");

            migrationBuilder.DropTable(
                name: "LogisticsManager");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_LogisticsManagerId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "LogisticsManagerId",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "clientPhoneNumber",
                table: "Clients",
                newName: "clientRegion");
        }
    }
}
