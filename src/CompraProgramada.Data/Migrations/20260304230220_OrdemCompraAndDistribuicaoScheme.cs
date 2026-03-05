using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrdemCompraAndDistribuicaoScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "considerado_nova_compra",
                table: "custodia_master");

            migrationBuilder.CreateTable(
                name: "ordem_compra",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ticker = table.Column<string>(type: "longtext", nullable: false, comment: "ativo comprado")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade_lote_padrao = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "quantos multiplos de 100 foram comprados"),
                    quantidade = table.Column<int>(type: "int", nullable: false, comment: "quantidade total de ativos comprados"),
                    preco_execucao = table.Column<decimal>(type: "decimal(65,30)", nullable: false, comment: "preço da compra executada"),
                    data = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "data da compra")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordem_compra", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "distribuicao",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ordem_compra_id = table.Column<int>(type: "int", nullable: false, comment: "identificador ordem de compra"),
                    conta_grafica_id = table.Column<int>(type: "int", nullable: false, comment: "identificador conta cliente"),
                    ticker = table.Column<string>(type: "longtext", nullable: false, comment: "ativo comprado")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade_alocada = table.Column<int>(type: "int", nullable: false, comment: "quantidade alocada na custodia"),
                    valor_operacao = table.Column<decimal>(type: "decimal(65,30)", nullable: false, comment: "resultado financeiro da fatia")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_distribuicao", x => x.id);
                    table.ForeignKey(
                        name: "FK_distribuicao_conta_grafica_conta_grafica_id",
                        column: x => x.conta_grafica_id,
                        principalTable: "conta_grafica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_distribuicao_ordem_compra_ordem_compra_id",
                        column: x => x.ordem_compra_id,
                        principalTable: "ordem_compra",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_distribuicao_conta_grafica_id",
                table: "distribuicao",
                column: "conta_grafica_id");

            migrationBuilder.CreateIndex(
                name: "IX_distribuicao_ordem_compra_id",
                table: "distribuicao",
                column: "ordem_compra_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "distribuicao");

            migrationBuilder.DropTable(
                name: "ordem_compra");

            migrationBuilder.AddColumn<bool>(
                name: "considerado_nova_compra",
                table: "custodia_master",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                comment: "status se já foi considerado na nova compra");
        }
    }
}
