using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Migrations
{
    /// <inheritdoc />
    public partial class removeadminIdFromStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Clients_ClientId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Stuffs_Admins_AdminId",
                table: "Stuffs");

            migrationBuilder.DropIndex(
                name: "IX_Stuffs_AdminId",
                table: "Stuffs");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Stuffs");

            migrationBuilder.AddColumn<string>(
                name: "AdminUserId",
                table: "Stuffs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Stuffs_AdminUserId",
                table: "Stuffs",
                column: "AdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Clients_ClientId",
                table: "Bookings",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stuffs_Admins_AdminUserId",
                table: "Stuffs",
                column: "AdminUserId",
                principalTable: "Admins",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Clients_ClientId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Stuffs_Admins_AdminUserId",
                table: "Stuffs");

            migrationBuilder.DropIndex(
                name: "IX_Stuffs_AdminUserId",
                table: "Stuffs");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "Stuffs");

            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "Stuffs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stuffs_AdminId",
                table: "Stuffs",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Clients_ClientId",
                table: "Bookings",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stuffs_Admins_AdminId",
                table: "Stuffs",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
