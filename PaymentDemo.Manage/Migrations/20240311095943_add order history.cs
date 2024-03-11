using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentDemo.Manage.Migrations
{
    /// <inheritdoc />
    public partial class addorderhistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderHistory",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderHistory",
                table: "Orders");
        }
    }
}
