using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class MotorCompraScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_custodia_filhote_conta_grafica_conta_grafica_id",
                table: "custodia_filhote");

            migrationBuilder.DropIndex(
                name: "IX_custodia_filhote_conta_grafica_id",
                table: "custodia_filhote");

            migrationBuilder.AddColumn<decimal>(
                name: "valor_anterior",
                table: "cliente",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "valor anterior de compra");

            migrationBuilder.CreateTable(
                name: "cesta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "longtext", nullable: false, comment: "nome cesta")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    data_desativacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ativa = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true, comment: "cesta ativa")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cesta", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cotacao",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    data_pregao = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "data do pregão")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cotacao", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "historico_execucao_motor",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    data_referencia = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "data que deveria ser executada"),
                    data_execucao = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "data que foi executado"),
                    executado = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true, comment: "compra feita")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_execucao_motor", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "composicao_cesta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cesta_id = table.Column<int>(type: "int", nullable: false, comment: "identificador cesta"),
                    ticker = table.Column<string>(type: "longtext", nullable: false, comment: "ativo")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    percentual = table.Column<decimal>(type: "decimal(65,30)", nullable: false, comment: "percentual ocupante na cesta")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_composicao_cesta", x => x.id);
                    table.ForeignKey(
                        name: "FK_composicao_cesta_cesta_cesta_id",
                        column: x => x.cesta_id,
                        principalTable: "cesta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "composicao_cotacao",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cotacao_id = table.Column<int>(type: "int", nullable: false, comment: "identificador composição de cotação"),
                    ticker = table.Column<string>(type: "longtext", nullable: false, comment: "identificação ativo")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    preco_fechamento = table.Column<decimal>(type: "decimal(65,30)", nullable: false, comment: "preço do fechamento do ativo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_composicao_cotacao", x => x.id);
                    table.ForeignKey(
                        name: "FK_composicao_cotacao_cotacao_cotacao_id",
                        column: x => x.cotacao_id,
                        principalTable: "cotacao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_custodia_filhote_conta_grafica_id",
                table: "custodia_filhote",
                column: "conta_grafica_id");

            migrationBuilder.AddForeignKey(
                name: "FK_custodia_filhote_conta_grafica_conta_grafica_id",
                table: "custodia_filhote",
                column: "conta_grafica_id",
                principalTable: "conta_grafica",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_composicao_cesta_cesta_id",
                table: "composicao_cesta",
                column: "cesta_id");

            migrationBuilder.CreateIndex(
                name: "IX_composicao_cotacao_cotacao_id",
                table: "composicao_cotacao",
                column: "cotacao_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "composicao_cesta");

            migrationBuilder.DropTable(
                name: "composicao_cotacao");

            migrationBuilder.DropTable(
                name: "historico_execucao_motor");

            migrationBuilder.DropTable(
                name: "cesta");

            migrationBuilder.DropTable(
                name: "cotacao");

            migrationBuilder.DropIndex(
                name: "IX_custodia_filhote_conta_grafica_id",
                table: "custodia_filhote");

            migrationBuilder.DropColumn(
                name: "valor_anterior",
                table: "cliente");

            migrationBuilder.CreateIndex(
                name: "IX_custodia_filhote_conta_grafica_id",
                table: "custodia_filhote",
                column: "conta_grafica_id",
                unique: true);
        }
    }
}
