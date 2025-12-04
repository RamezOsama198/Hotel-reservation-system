using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Migrations
{
    /// <inheritdoc />
    public partial class v14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stuffs_Admins_AdminUserId",
                table: "Stuffs");

            migrationBuilder.DropTable(
                name: "AdminBooking");

            migrationBuilder.DropTable(
                name: "AdminComment");

            migrationBuilder.DropIndex(
                name: "IX_Stuffs_AdminUserId",
                table: "Stuffs");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "Stuffs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminUserId",
                table: "Stuffs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminBooking",
                columns: table => new
                {
                    AdminsUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminBooking", x => new { x.AdminsUserId, x.BookingsId });
                    table.ForeignKey(
                        name: "FK_AdminBooking_Admins_AdminsUserId",
                        column: x => x.AdminsUserId,
                        principalTable: "Admins",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminBooking_Bookings_BookingsId",
                        column: x => x.BookingsId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminComment",
                columns: table => new
                {
                    AdminsUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminComment", x => new { x.AdminsUserId, x.CommentsId });
                    table.ForeignKey(
                        name: "FK_AdminComment_Admins_AdminsUserId",
                        column: x => x.AdminsUserId,
                        principalTable: "Admins",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminComment_Comments_CommentsId",
                        column: x => x.CommentsId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stuffs_AdminUserId",
                table: "Stuffs",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminBooking_BookingsId",
                table: "AdminBooking",
                column: "BookingsId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminComment_CommentsId",
                table: "AdminComment",
                column: "CommentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stuffs_Admins_AdminUserId",
                table: "Stuffs",
                column: "AdminUserId",
                principalTable: "Admins",
                principalColumn: "UserId");
        }
    }
}
