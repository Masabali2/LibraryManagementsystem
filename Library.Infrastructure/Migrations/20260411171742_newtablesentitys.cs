using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newtablesentitys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Shelf_ShelfId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Journals_Shelf_ShelfId",
                table: "Journals");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelf_LocationBlock_LocationBlockId",
                table: "Shelf");

            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Shelf_ShelfId",
                table: "Theses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shelf",
                table: "Shelf");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationBlock",
                table: "LocationBlock");

            migrationBuilder.RenameTable(
                name: "Shelf",
                newName: "shelf");

            migrationBuilder.RenameTable(
                name: "LocationBlock",
                newName: "LocationBlocks");

            migrationBuilder.RenameIndex(
                name: "IX_Shelf_LocationBlockId",
                table: "shelf",
                newName: "IX_shelf_LocationBlockId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shelf",
                table: "shelf",
                column: "ShelfId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationBlocks",
                table: "LocationBlocks",
                column: "LocationBlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_shelf_ShelfId",
                table: "Books",
                column: "ShelfId",
                principalTable: "shelf",
                principalColumn: "ShelfId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journals_shelf_ShelfId",
                table: "Journals",
                column: "ShelfId",
                principalTable: "shelf",
                principalColumn: "ShelfId");

            migrationBuilder.AddForeignKey(
                name: "FK_shelf_LocationBlocks_LocationBlockId",
                table: "shelf",
                column: "LocationBlockId",
                principalTable: "LocationBlocks",
                principalColumn: "LocationBlockId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_shelf_ShelfId",
                table: "Theses",
                column: "ShelfId",
                principalTable: "shelf",
                principalColumn: "ShelfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_shelf_ShelfId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Journals_shelf_ShelfId",
                table: "Journals");

            migrationBuilder.DropForeignKey(
                name: "FK_shelf_LocationBlocks_LocationBlockId",
                table: "shelf");

            migrationBuilder.DropForeignKey(
                name: "FK_Theses_shelf_ShelfId",
                table: "Theses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shelf",
                table: "shelf");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationBlocks",
                table: "LocationBlocks");

            migrationBuilder.RenameTable(
                name: "shelf",
                newName: "Shelf");

            migrationBuilder.RenameTable(
                name: "LocationBlocks",
                newName: "LocationBlock");

            migrationBuilder.RenameIndex(
                name: "IX_shelf_LocationBlockId",
                table: "Shelf",
                newName: "IX_Shelf_LocationBlockId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shelf",
                table: "Shelf",
                column: "ShelfId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationBlock",
                table: "LocationBlock",
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
                name: "FK_Shelf_LocationBlock_LocationBlockId",
                table: "Shelf",
                column: "LocationBlockId",
                principalTable: "LocationBlock",
                principalColumn: "LocationBlockId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Shelf_ShelfId",
                table: "Theses",
                column: "ShelfId",
                principalTable: "Shelf",
                principalColumn: "ShelfId");
        }
    }
}
