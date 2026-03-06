using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginationMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Product 1", 15.00m },
                    { 2, "Product 2", 20.00m },
                    { 3, "Product 3", 25.00m },
                    { 4, "Product 4", 30.00m },
                    { 5, "Product 5", 35.00m },
                    { 6, "Product 6", 40.00m },
                    { 7, "Product 7", 45.00m },
                    { 8, "Product 8", 50.00m },
                    { 9, "Product 9", 55.00m },
                    { 10, "Product 10", 60.00m },
                    { 11, "Product 11", 65.00m },
                    { 12, "Product 12", 70.00m },
                    { 13, "Product 13", 75.00m },
                    { 14, "Product 14", 80.00m },
                    { 15, "Product 15", 85.00m },
                    { 16, "Product 16", 90.00m },
                    { 17, "Product 17", 95.00m },
                    { 18, "Product 18", 100.00m },
                    { 19, "Product 19", 105.00m },
                    { 20, "Product 20", 110.00m },
                    { 21, "Product 21", 115.00m },
                    { 22, "Product 22", 120.00m },
                    { 23, "Product 23", 125.00m },
                    { 24, "Product 24", 130.00m },
                    { 25, "Product 25", 135.00m },
                    { 26, "Product 26", 140.00m },
                    { 27, "Product 27", 145.00m },
                    { 28, "Product 28", 150.00m },
                    { 29, "Product 29", 155.00m },
                    { 30, "Product 30", 160.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
