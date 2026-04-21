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
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_LogisticsManager_LogisticsManagerId",
                table: "Contracts");

            migrationBuilder.AlterColumn<int>(
                name: "LogisticsManagerId",
                table: "Contracts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_LogisticsManager_LogisticsManagerId",
                table: "Contracts",
                column: "LogisticsManagerId",
                principalTable: "LogisticsManager",
                principalColumn: "LogisticsManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_LogisticsManager_LogisticsManagerId",
                table: "Contracts");

            migrationBuilder.AlterColumn<int>(
                name: "LogisticsManagerId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_LogisticsManager_LogisticsManagerId",
                table: "Contracts",
                column: "LogisticsManagerId",
                principalTable: "LogisticsManager",
                principalColumn: "LogisticsManagerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
