using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatesql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BorrowingRecords_ItemId",
                table: "BorrowingRecords",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingRecords_Books_ItemId",
                table: "BorrowingRecords",
                column: "ItemId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingRecords_Books_ItemId",
                table: "BorrowingRecords");

            migrationBuilder.DropIndex(
                name: "IX_BorrowingRecords_ItemId",
                table: "BorrowingRecords");
        }
    }
}
