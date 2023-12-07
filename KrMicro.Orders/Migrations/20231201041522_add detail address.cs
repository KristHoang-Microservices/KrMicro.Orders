using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KrMicro.Orders.Migrations
{
    public partial class adddetailaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "CityId",
                table: "DeliveryInformation",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "DistrictId",
                table: "DeliveryInformation",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "WardId",
                table: "DeliveryInformation",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "DeliveryInformation");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "DeliveryInformation");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "DeliveryInformation");
        }
    }
}
