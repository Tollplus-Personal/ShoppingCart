using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingCart.Data.Migrations
{
    public partial class vamsi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Qty = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    ProductName = table.Column<string>(nullable: true),
                    FileImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Order15",
                columns: table => new
                {
                    OrderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderAmount = table.Column<decimal>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order15", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "Product15",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(nullable: false),
                    ProductQuantity = table.Column<int>(nullable: false),
                    ProductPrice = table.Column<decimal>(nullable: false),
                    FileImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product15", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail15",
                columns: table => new
                {
                    OrderdetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail15", x => x.OrderdetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetail15_Order15_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order15",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail15_Product15_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product15",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail15_OrderId",
                table: "OrderDetail15",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail15_ProductId",
                table: "OrderDetail15",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "OrderDetail15");

            migrationBuilder.DropTable(
                name: "Order15");

            migrationBuilder.DropTable(
                name: "Product15");
        }
    }
}
