using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaTipoOrdemCompraColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "tipo",
                table: "ordem_compra_detalhe",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "tipo do lote",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "tipo do lote")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tipo",
                table: "ordem_compra_detalhe",
                type: "longtext",
                nullable: false,
                comment: "tipo do lote",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1,
                oldComment: "tipo do lote")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
