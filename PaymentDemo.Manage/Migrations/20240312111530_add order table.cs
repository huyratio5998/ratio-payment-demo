using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentDemo.Manage.Migrations
{
    /// <inheritdoc />
    public partial class addordertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentProvider",
                table: "Orders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "Orders");
        }
    }
}
