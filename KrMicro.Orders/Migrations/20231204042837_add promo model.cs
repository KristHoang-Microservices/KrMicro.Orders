using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KrMicro.Orders.Migrations
{
    public partial class addpromomodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "PromoId",
                table: "Orders",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Promo",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    PromoUnit = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PromoId",
                table: "Orders",
                column: "PromoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Promo_PromoId",
                table: "Orders",
                column: "PromoId",
                principalTable: "Promo",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Promo_PromoId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Promo");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PromoId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PromoId",
                table: "Orders");
        }
    }
}
