using Microsoft.EntityFrameworkCore.Migrations;

namespace DeliveryApp.DataLayer.Migrations
{
    public partial class PositionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Occupancy",
                table: "Vehicles",
                newName: "Load");

            migrationBuilder.AddColumn<int>(
                name: "AverageSpeed",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiverPositionId",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PositionId",
                table: "Users",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ReceiverPositionId",
                table: "Packages",
                column: "ReceiverPositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Position_ReceiverPositionId",
                table: "Packages",
                column: "ReceiverPositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Position_PositionId",
                table: "Users",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Position_ReceiverPositionId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Position_PositionId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropIndex(
                name: "IX_Users_PositionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Packages_ReceiverPositionId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "AverageSpeed",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReceiverPositionId",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "Load",
                table: "Vehicles",
                newName: "Occupancy");
        }
    }
}
