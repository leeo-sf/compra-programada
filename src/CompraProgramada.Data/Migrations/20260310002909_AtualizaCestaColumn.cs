using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaCestaColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "custodia_filhote",
                keyColumn: "ticker",
                keyValue: null,
                column: "ticker",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ticker",
                table: "custodia_filhote",
                type: "longtext",
                nullable: false,
                comment: "ativo",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "ativo")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_desativacao",
                table: "cesta",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ticker",
                table: "custodia_filhote",
                type: "longtext",
                nullable: true,
                comment: "ativo",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "ativo")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_desativacao",
                table: "cesta",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
