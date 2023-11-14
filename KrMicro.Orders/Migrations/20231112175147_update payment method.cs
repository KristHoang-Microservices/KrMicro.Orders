using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KrMicro.Orders.Migrations
{
    public partial class updatepaymentmethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "PaymentMethods",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PaymentMethods",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PaymentMethods");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PaymentMethods",
                newName: "name");
        }
    }
}
