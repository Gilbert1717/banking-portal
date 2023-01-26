using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McbaSystem.Migrations
{
    /// <inheritdoc />
    public partial class addErrorMessageToBillPay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "BillPays",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "BillPays");
        }
    }
}
