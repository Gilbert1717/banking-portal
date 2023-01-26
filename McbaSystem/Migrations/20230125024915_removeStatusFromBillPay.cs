using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McbaSystem.Migrations
{
    /// <inheritdoc />
    public partial class removeStatusFromBillPay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "BillPays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BillPays",
                type: "char(1)",
                nullable: false,
                defaultValue: "");
        }
    }
}
