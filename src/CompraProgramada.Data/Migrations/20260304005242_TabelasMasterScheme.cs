using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class TabelasMasterScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conta_master",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tipo = table.Column<string>(type: "longtext", nullable: false, comment: "tipo conta")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_conta = table.Column<string>(type: "longtext", nullable: false, comment: "número conta")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conta_master", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "custodia_master",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "identificador")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    conta_master_id = table.Column<int>(type: "int", nullable: false, comment: "identificador conta"),
                    ticker = table.Column<string>(type: "longtext", nullable: false, comment: "ativo")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade_residuo = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "quantidade de ativos que sobraram"),
                    considerado_nova_compra = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false, comment: "status se já foi considerado na nova compra")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custodia_master", x => x.id);
                    table.ForeignKey(
                        name: "FK_custodia_master_conta_master_conta_master_id",
                        column: x => x.conta_master_id,
                        principalTable: "conta_master",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_custodia_master_conta_master_id",
                table: "custodia_master",
                column: "conta_master_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "custodia_master");

            migrationBuilder.DropTable(
                name: "conta_master");
        }
    }
}
