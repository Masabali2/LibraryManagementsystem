using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newtablesentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShelfId",
                table: "Theses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShelfId",
                table: "Journals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShelfId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocationBlock",
                columns: table => new
                {
                    LocationBlockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationBlock", x => x.LocationBlockId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Shelf",
                columns: table => new
                {
                    ShelfId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShelfCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LocationBlockId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shelf", x => x.ShelfId);
                    table.ForeignKey(
                        name: "FK_Shelf_LocationBlock_LocationBlockId",
                        column: x => x.LocationBlockId,
                        principalTable: "LocationBlock",
                        principalColumn: "LocationBlockId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Theses_ShelfId",
                table: "Theses",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_ShelfId",
                table: "Journals",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ShelfId",
                table: "Books",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_Shelf_LocationBlockId",
                table: "Shelf",
                column: "LocationBlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Shelf_ShelfId",
                table: "Books",
                column: "ShelfId",
                principalTable: "Shelf",
                principalColumn: "ShelfId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journals_Shelf_ShelfId",
                table: "Journals",
                column: "ShelfId",
                principalTable: "Shelf",
                principalColumn: "ShelfId");

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Shelf_ShelfId",
                table: "Theses",
                column: "ShelfId",
                principalTable: "Shelf",
                principalColumn: "ShelfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Shelf_ShelfId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Journals_Shelf_ShelfId",
                table: "Journals");

            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Shelf_ShelfId",
                table: "Theses");

            migrationBuilder.DropTable(
                name: "Shelf");

            migrationBuilder.DropTable(
                name: "LocationBlock");

            migrationBuilder.DropIndex(
                name: "IX_Theses_ShelfId",
                table: "Theses");

            migrationBuilder.DropIndex(
                name: "IX_Journals_ShelfId",
                table: "Journals");

            migrationBuilder.DropIndex(
                name: "IX_Books_ShelfId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ShelfId",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "ShelfId",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "ShelfId",
                table: "Books");
        }
    }
}
