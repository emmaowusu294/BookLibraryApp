using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibraryApp.Migrations
{
    /// <inheritdoc />
    public partial class DigitalLoanConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Patrons_PatronId",
                table: "Loans");

            migrationBuilder.DropTable(
                name: "Patrons");

            migrationBuilder.DropIndex(
                name: "IX_Loans_PatronId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PatronId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "CheckoutDate",
                table: "Loans",
                newName: "LoanDate");

            migrationBuilder.RenameColumn(
                name: "LoanId",
                table: "Loans",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // 🔑 CRITICAL FIX: Delete existing loan data before adding the new required FK column
            migrationBuilder.Sql("DELETE FROM [Loans]");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UserId",
                table: "Loans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_UserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "LoanDate",
                table: "Loans",
                newName: "CheckoutDate");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Loans",
                newName: "LoanId");

            migrationBuilder.AddColumn<int>(
                name: "PatronId",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "Loans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Patrons",
                columns: table => new
                {
                    PatronId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrons", x => x.PatronId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_PatronId",
                table: "Loans",
                column: "PatronId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Patrons_PatronId",
                table: "Loans",
                column: "PatronId",
                principalTable: "Patrons",
                principalColumn: "PatronId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}