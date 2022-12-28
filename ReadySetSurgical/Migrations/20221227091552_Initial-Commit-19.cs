using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadySetSurgical.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_logs",
                table: "logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_details",
                table: "details");

            migrationBuilder.RenameTable(
                name: "logs",
                newName: "errorlog");

            migrationBuilder.RenameTable(
                name: "details",
                newName: "invoiceDetail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_errorlog",
                table: "errorlog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invoiceDetail",
                table: "invoiceDetail",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_invoiceDetail",
                table: "invoiceDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_errorlog",
                table: "errorlog");

            migrationBuilder.RenameTable(
                name: "invoiceDetail",
                newName: "details");

            migrationBuilder.RenameTable(
                name: "errorlog",
                newName: "logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_details",
                table: "details",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_logs",
                table: "logs",
                column: "Id");
        }
    }
}
