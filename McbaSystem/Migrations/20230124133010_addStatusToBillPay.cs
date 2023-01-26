using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McbaSystem.Migrations
{
    /// <inheritdoc />
    public partial class addStatusToBillPay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "Transactions",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char");

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                table: "BillPays",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BillPays",
                type: "char(1)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CH_BillPay_Amount",
                table: "BillPays",
                sql: "Amount > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CH_BillPay_Amount",
                table: "BillPays");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BillPays");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "Transactions",
                type: "char",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                table: "BillPays",
                type: "char",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)");
        }
    }
}
