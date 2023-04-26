using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCareForAll.Migrations
{
    /// <inheritdoc />
    public partial class cartadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    itemid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    drugsId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carts", x => x.itemid);
                    table.ForeignKey(
                        name: "FK_carts_drugs_drugsId",
                        column: x => x.drugsId,
                        principalTable: "drugs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_carts_drugsId",
                table: "carts",
                column: "drugsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "carts");
        }
    }
}
