using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _2World.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Image_Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Category_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Category_Category_Id",
                        column: x => x.Category_Id,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Delivery_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Customer_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Customer_Customer_Id",
                        column: x => x.Customer_Id,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role_Id = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_Role_Id",
                        column: x => x.Role_Id,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Order_Id = table.Column<int>(type: "int", nullable: false),
                    Product_Id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => new { x.Order_Id, x.Product_Id });
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_Order_Id",
                        column: x => x.Order_Id,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItem_Product_Product_Id",
                        column: x => x.Product_Id,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Created_At", "Deleted_At", "Name", "Updated_At" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4010), null, "Books", new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4011) },
                    { 2, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4014), null, "Electronics", new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4014) },
                    { 3, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4015), null, "Clothes", new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4016) }
                });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "Address", "Created_At", "Deleted_At", "Email", "Name", "Password", "Phone", "Updated_At" },
                values: new object[,]
                {
                    { 1, "Kozeyview", new DateTime(2024, 7, 14, 20, 37, 51, 351, DateTimeKind.Local).AddTicks(6342), null, "Imelda@gmail.com", "Ms. Colby Rosenbaum", "$2a$13$sH9ro5N6GSlZyq9HcQ5WXe2Hz87tt0KTAwUx1M/E2KBwYUl.xwMg2", "1-320-979-3192 x71343", new DateTime(2024, 7, 14, 20, 37, 51, 351, DateTimeKind.Local).AddTicks(6348) },
                    { 2, "Briachester", new DateTime(2024, 7, 14, 20, 37, 51, 871, DateTimeKind.Local).AddTicks(6449), null, "Damian@gmail.com", "Miss Sydnee Ahmed Treutel", "$2a$13$jqRkkSrOG82i.eXkzPfJOuOsiipBjsQO79OQSuONfIaAdIhBGDwPi", "(281)594-1910", new DateTime(2024, 7, 14, 20, 37, 51, 871, DateTimeKind.Local).AddTicks(6455) },
                    { 3, "New Harrison", new DateTime(2024, 7, 14, 20, 37, 52, 381, DateTimeKind.Local).AddTicks(2771), null, "Dina@gmail.com", "Miss Dorothea Eloy Gerlach", "$2a$13$ypxDNHDLV9s/PlZKEbspw.fdWSe2l/PrpyFyTZ0ndNkuJfdYcDy12", "(183)802-0356", new DateTime(2024, 7, 14, 20, 37, 52, 381, DateTimeKind.Local).AddTicks(2775) },
                    { 4, "Laurineland", new DateTime(2024, 7, 14, 20, 37, 52, 911, DateTimeKind.Local).AddTicks(7048), null, "Kailyn@gmail.com", "Mrs. Violette Caesar Murazik", "$2a$13$19.jlTr/sr2EbBFRUch6hONz.6moMEzCmfqiYBP1Oy0ZL./Y0.HqC", "309-835-5595 x7631", new DateTime(2024, 7, 14, 20, 37, 52, 911, DateTimeKind.Local).AddTicks(7053) },
                    { 5, "Port Chelsie", new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(3633), null, "Rowland@gmail.com", "Rosella Bradtke", "$2a$13$pyG3U.6UhiUcdnpHk71uBu2lvIpKtWu9kv/UgglUtVDHayvehzed2", "(504)568-7113 x59651", new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(3638) }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Created_At", "Deleted_At", "Name", "Updated_At" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 7, 14, 20, 37, 50, 319, DateTimeKind.Local).AddTicks(3208), null, "ADMIN", new DateTime(2024, 7, 14, 20, 37, 50, 319, DateTimeKind.Local).AddTicks(3219) },
                    { 2, new DateTime(2024, 7, 14, 20, 37, 50, 319, DateTimeKind.Local).AddTicks(3221), null, "USER", new DateTime(2024, 7, 14, 20, 37, 50, 319, DateTimeKind.Local).AddTicks(3221) },
                    { 3, new DateTime(2024, 7, 14, 20, 37, 50, 319, DateTimeKind.Local).AddTicks(3222), null, "GUEST", new DateTime(2024, 7, 14, 20, 37, 50, 319, DateTimeKind.Local).AddTicks(3223) }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "Id", "Customer_Id", "Delivery_Date", "Order_Date", "Status" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 2, new DateTime(2024, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 2, new DateTime(2024, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, 5, null, new DateTime(2024, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 5, 4, null, new DateTime(2024, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 6, 3, new DateTime(2023, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "Category_Id", "Created_At", "Deleted_At", "Description", "Image_Path", "Name", "Price", "Quantity", "Updated_At" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4079), null, null, null, "Product 1", 10m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4080) },
                    { 2, 2, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4082), null, null, null, "Product 2", 12m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4082) },
                    { 3, 1, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4084), null, null, null, "Product 3", 13m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4084) },
                    { 4, 3, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4085), null, null, null, "Product 4", 14m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4086) },
                    { 5, 3, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4087), null, null, null, "Product 5", 15m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4087) },
                    { 6, 1, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4089), null, null, null, "Product 6", 16m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4089) },
                    { 7, 2, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4090), null, null, null, "Product 7", 17m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4090) },
                    { 8, 2, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4092), null, null, null, "Product 8", 18m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4092) },
                    { 9, 1, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4093), null, null, null, "Product 9", 19m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4094) },
                    { 10, 3, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4095), null, null, null, "Product 10", 20m, 12, new DateTime(2024, 7, 14, 20, 37, 53, 435, DateTimeKind.Local).AddTicks(4095) }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Created_At", "Deleted_At", "Email", "Name", "Password", "Phone", "Role_Id", "Updated_At" },
                values: new object[] { 1, new DateTime(2024, 7, 14, 20, 37, 50, 828, DateTimeKind.Local).AddTicks(7782), null, "admin@gmail.com", "Admin", "$2a$13$15FZTW2OoL1nUWJC0h7df.Tp2X8e43pPZlO8dbOeDWe7vBG2Uvp8u", "0987654321", 1, new DateTime(2024, 7, 14, 20, 37, 50, 828, DateTimeKind.Local).AddTicks(7788) });

            migrationBuilder.InsertData(
                table: "OrderItem",
                columns: new[] { "Order_Id", "Product_Id", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 11, 1 },
                    { 2, 1, 11, 1 },
                    { 2, 2, 12, 1 },
                    { 3, 4, 14, 1 },
                    { 4, 1, 11, 1 },
                    { 4, 2, 12, 1 },
                    { 4, 4, 14, 1 },
                    { 5, 3, 13, 1 },
                    { 6, 1, 11, 1 },
                    { 6, 10, 20, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Customer_Id",
                table: "Order",
                column: "Customer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_Product_Id",
                table: "OrderItem",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Category_Id",
                table: "Product",
                column: "Category_Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Role_Id",
                table: "User",
                column: "Role_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
