using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadySetSurgical.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_invoiceDetail",
                table: "invoiceDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_errorlog",
                table: "errorlog");

            migrationBuilder.RenameTable(
                name: "invoiceDetail",
                newName: "InvoiceDetail");

            migrationBuilder.RenameTable(
                name: "errorlog",
                newName: "Errorlog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceDetail",
                table: "InvoiceDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Errorlog",
                table: "Errorlog",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceDetail",
                table: "InvoiceDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Errorlog",
                table: "Errorlog");

            migrationBuilder.RenameTable(
                name: "InvoiceDetail",
                newName: "invoiceDetail");

            migrationBuilder.RenameTable(
                name: "Errorlog",
                newName: "errorlog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invoiceDetail",
                table: "invoiceDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_errorlog",
                table: "errorlog",
                column: "Id");
        }
    }
}
