using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrdemCompraDetalhesScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "preco_execucao",
                table: "ordem_compra");

            migrationBuilder.DropColumn(
                name: "quantidade",
                table: "ordem_compra");

            migrationBuilder.DropColumn(
                name: "quantidade_lote_padrao",
                table: "ordem_compra");

            migrationBuilder.AddColumn<decimal>(
                name: "preco_unitatio",
                table: "ordem_compra",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "preço de fechamento de cada ativo");

            migrationBuilder.AddColumn<int>(
                name: "quantidade_total",
                table: "ordem_compra",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "quantos total de ativos");

            migrationBuilder.AddColumn<decimal>(
                name: "valor_total",
                table: "ordem_compra",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "valor total da ordem de compra");

            migrationBuilder.CreateTable(
                name: "ordem_compra_detalhe",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tipo = table.Column<string>(type: "longtext", nullable: false, comment: "tipo do lote")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ticker = table.Column<string>(type: "longtext", nullable: false, comment: "ativo comprado")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "quantidade do lote"),
                    ordem_compra_id = table.Column<int>(type: "int", nullable: false, comment: "identificador ordem compra")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordem_compra_detalhe", x => x.id);
                    table.ForeignKey(
                        name: "FK_ordem_compra_detalhe_ordem_compra_ordem_compra_id",
                        column: x => x.ordem_compra_id,
                        principalTable: "ordem_compra",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ordem_compra_detalhe_ordem_compra_id",
                table: "ordem_compra_detalhe",
                column: "ordem_compra_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ordem_compra_detalhe");

            migrationBuilder.DropColumn(
                name: "preco_unitatio",
                table: "ordem_compra");

            migrationBuilder.DropColumn(
                name: "quantidade_total",
                table: "ordem_compra");

            migrationBuilder.DropColumn(
                name: "valor_total",
                table: "ordem_compra");

            migrationBuilder.AddColumn<decimal>(
                name: "preco_execucao",
                table: "ordem_compra",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "preço da compra executada");

            migrationBuilder.AddColumn<int>(
                name: "quantidade",
                table: "ordem_compra",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "quantidade total de ativos comprados");

            migrationBuilder.AddColumn<int>(
                name: "quantidade_lote_padrao",
                table: "ordem_compra",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "quantos multiplos de 100 foram comprados");
        }
    }
}
