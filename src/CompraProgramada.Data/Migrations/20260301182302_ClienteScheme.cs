using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClienteScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "longtext", nullable: false, comment: "nome usuário")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false, comment: "cpf do usuário")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false, comment: "email usuário")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor_mensal = table.Column<decimal>(type: "decimal(65,30)", nullable: false, comment: "valor mensal de compra"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true, comment: "usuário ativo"),
                    data_adesao = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cliente", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "conta_grafica",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<string>(type: "longtext", nullable: false, comment: "tipo conta")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_conta = table.Column<string>(type: "longtext", nullable: false, comment: "número conta")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conta_grafica", x => x.id);
                    table.ForeignKey(
                        name: "FK_conta_grafica_cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "custodia_filhote",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    conta_grafica_id = table.Column<int>(type: "int", nullable: false, comment: "identificador conta"),
                    ticker = table.Column<string>(type: "longtext", nullable: true, comment: "ativo")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "quantidade comprado")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custodia_filhote", x => x.id);
                    table.ForeignKey(
                        name: "FK_custodia_filhote_conta_grafica_conta_grafica_id",
                        column: x => x.conta_grafica_id,
                        principalTable: "conta_grafica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_cliente_cpf",
                table: "cliente",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_conta_grafica_ClienteId",
                table: "conta_grafica",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_custodia_filhote_conta_grafica_id",
                table: "custodia_filhote",
                column: "conta_grafica_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "custodia_filhote");

            migrationBuilder.DropTable(
                name: "conta_grafica");

            migrationBuilder.DropTable(
                name: "cliente");
        }
    }
}
