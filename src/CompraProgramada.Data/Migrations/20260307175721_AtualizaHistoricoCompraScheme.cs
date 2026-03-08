using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaHistoricoCompraScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evolucao_carteira");

            migrationBuilder.DropColumn(
                name: "valor",
                table: "historico_compra");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "historico_compra",
                newName: "Data");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Data",
                table: "historico_compra",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldComment: "data da compra");

            migrationBuilder.AddColumn<decimal>(
                name: "preco_executado",
                table: "historico_compra",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "preco de fechamento do ativo");

            migrationBuilder.AddColumn<decimal>(
                name: "preco_medio",
                table: "historico_compra",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "preco medio");

            migrationBuilder.AddColumn<int>(
                name: "quantidade",
                table: "historico_compra",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "quantidade comprado");

            migrationBuilder.AddColumn<string>(
                name: "ticker",
                table: "historico_compra",
                type: "longtext",
                nullable: false,
                comment: "ativo comprado")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "valor_aporte",
                table: "historico_compra",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "valor aporte");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "preco_executado",
                table: "historico_compra");

            migrationBuilder.DropColumn(
                name: "preco_medio",
                table: "historico_compra");

            migrationBuilder.DropColumn(
                name: "quantidade",
                table: "historico_compra");

            migrationBuilder.DropColumn(
                name: "ticker",
                table: "historico_compra");

            migrationBuilder.DropColumn(
                name: "valor_aporte",
                table: "historico_compra");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "historico_compra",
                newName: "data");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "data",
                table: "historico_compra",
                type: "date",
                nullable: false,
                comment: "data da compra",
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<decimal>(
                name: "valor",
                table: "historico_compra",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "valor compra");

            migrationBuilder.CreateTable(
                name: "evolucao_carteira",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    conta_grafica_id = table.Column<int>(type: "int", nullable: false, comment: "identificador conta"),
                    data = table.Column<DateOnly>(type: "date", nullable: false, comment: "data da compra"),
                    rentabilidade = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    valor_carteira = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    valor_investido = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_evolucao_carteira_conta_grafica_id",
                table: "evolucao_carteira",
                column: "conta_grafica_id");
        }
    }
}
