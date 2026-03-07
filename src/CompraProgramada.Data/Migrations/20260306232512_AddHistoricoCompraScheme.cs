using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricoCompraScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evolucao_carteira",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    data = table.Column<DateOnly>(type: "date", nullable: false, comment: "data da compra"),
                    valor_carteira = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    valor_investido = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    rentabilidade = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    conta_grafica_id = table.Column<int>(type: "int", nullable: false, comment: "identificador conta")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evolucao_carteira", x => x.id);
                    table.ForeignKey(
                        name: "FK_evolucao_carteira_conta_grafica_conta_grafica_id",
                        column: x => x.conta_grafica_id,
                        principalTable: "conta_grafica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "historico_compra",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    data = table.Column<DateOnly>(type: "date", nullable: false, comment: "data da compra"),
                    valor = table.Column<decimal>(type: "decimal(65,30)", nullable: false, comment: "valor compra"),
                    conta_grafica_id = table.Column<int>(type: "int", nullable: false, comment: "identificador conta")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_compra", x => x.id);
                    table.ForeignKey(
                        name: "FK_historico_compra_conta_grafica_conta_grafica_id",
                        column: x => x.conta_grafica_id,
                        principalTable: "conta_grafica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_evolucao_carteira_conta_grafica_id",
                table: "evolucao_carteira",
                column: "conta_grafica_id");

            migrationBuilder.CreateIndex(
                name: "IX_historico_compra_conta_grafica_id",
                table: "historico_compra",
                column: "conta_grafica_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evolucao_carteira");

            migrationBuilder.DropTable(
                name: "historico_compra");
        }
    }
}
