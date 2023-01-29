using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McbaSystem.Migrations
{
    /// <inheritdoc />
    public partial class addBoolFlagToLoginAndBillPay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isLocked",
                table: "Logins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isBlocked",
                table: "BillPays",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isLocked",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "isBlocked",
                table: "BillPays");
        }
    }
}
