using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cesta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false, comment: "nome cesta"),
                    ativa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "cesta ativa"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de criação"),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de atualização")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cesta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false, comment: "nome usuário"),
                    cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false, comment: "cpf do usuário"),
                    email = table.Column<string>(type: "text", nullable: false, comment: "email usuário"),
                    valor_anterior = table.Column<decimal>(type: "numeric", nullable: false, comment: "valor anterior de compra"),
                    valor_mensal = table.Column<decimal>(type: "numeric", nullable: false, comment: "valor mensal de compra"),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "usuário ativo"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de criação"),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de atualização")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cliente", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "conta_master",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo = table.Column<string>(type: "text", nullable: false, comment: "tipo da conta"),
                    numero_conta = table.Column<string>(type: "text", nullable: false, comment: "número da conta"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conta_master", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cotacao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_pregao = table.Column<DateOnly>(type: "date", nullable: false, comment: "data do pregão"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de criação"),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de atualização")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cotacao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "historico_execucao_motor",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_referencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data que deveria ser executada"),
                    data_execucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data que foi executado"),
                    executado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "compra feita")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_execucao_motor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ordem_compra",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ticker = table.Column<string>(type: "text", nullable: false, comment: "ativo comprado"),
                    quantidade_total = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "quantos total de ativos"),
                    preco_unitatio = table.Column<decimal>(type: "numeric", nullable: false, comment: "preço de fechamento de cada ativo"),
                    valor_total = table.Column<decimal>(type: "numeric", nullable: false, comment: "valor total da ordem de compra"),
                    data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data da compra")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordem_compra", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "composicao_cesta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cesta_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador cesta"),
                    ticker = table.Column<string>(type: "text", nullable: false, comment: "ativo"),
                    percentual = table.Column<decimal>(type: "numeric", nullable: false, comment: "percentual ocupante na cesta")
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
                });

            migrationBuilder.CreateTable(
                name: "conta_grafica",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cliente_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador cliente"),
                    tipo = table.Column<string>(type: "text", nullable: false, comment: "tipo conta"),
                    numero_conta = table.Column<string>(type: "text", nullable: false, comment: "número da conta"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conta_grafica", x => x.id);
                    table.ForeignKey(
                        name: "FK_conta_grafica_cliente_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "custodia_master",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    conta_master_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador conta"),
                    ticker = table.Column<string>(type: "text", nullable: false, comment: "ativo"),
                    quantidade_residuo = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "quantidade de ativos que sobraram"),
                    preco_medio = table.Column<decimal>(type: "numeric", nullable: false, comment: "preço médio dos ativos"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de criação"),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de atualização")
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
                });

            migrationBuilder.CreateTable(
                name: "composicao_cotacao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cotacao_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador composição de cotação"),
                    ticker = table.Column<string>(type: "text", nullable: false, comment: "identificação ativo"),
                    preco_fechamento = table.Column<decimal>(type: "numeric", nullable: false, comment: "preço do fechamento do ativo")
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
                });

            migrationBuilder.CreateTable(
                name: "ordem_compra_detalhe",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo = table.Column<int>(type: "integer", nullable: false, comment: "tipo do lote"),
                    ticker = table.Column<string>(type: "text", nullable: false, comment: "ativo comprado"),
                    quantidade = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "quantidade do lote"),
                    ordem_compra_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador ordem compra")
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
                });

            migrationBuilder.CreateTable(
                name: "custodia_filhote",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    conta_grafica_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador conta"),
                    ticker = table.Column<string>(type: "text", nullable: false, comment: "ativo"),
                    preco_medio = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m, comment: "custo médio ponderado de compra"),
                    quantidade = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "quantidade comprado"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de criação"),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "data de atualização")
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
                });

            migrationBuilder.CreateTable(
                name: "distribuicao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ordem_compra_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador ordem de compra"),
                    conta_grafica_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador conta cliente"),
                    ticker = table.Column<string>(type: "text", nullable: false, comment: "ativo comprado"),
                    quantidade_alocada = table.Column<int>(type: "integer", nullable: false, comment: "quantidade alocada na custodia"),
                    valor_operacao = table.Column<decimal>(type: "numeric", nullable: false, comment: "resultado financeiro da fatia")
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
                });

            migrationBuilder.CreateTable(
                name: "historico_compra",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "identificador")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    conta_grafica_id = table.Column<int>(type: "integer", nullable: false, comment: "identificador conta"),
                    ticker = table.Column<string>(type: "text", nullable: false, comment: "ativo comprado"),
                    quantidade = table.Column<int>(type: "integer", nullable: false, comment: "quantidade comprado"),
                    valor_aporte = table.Column<decimal>(type: "numeric", nullable: false, comment: "valor aporte"),
                    preco_executado = table.Column<decimal>(type: "numeric", nullable: false, comment: "preco de fechamento do ativo"),
                    preco_medio = table.Column<decimal>(type: "numeric", nullable: false, comment: "preco medio"),
                    Data = table.Column<DateOnly>(type: "date", nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_cliente_cpf",
                table: "cliente",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_composicao_cesta_cesta_id",
                table: "composicao_cesta",
                column: "cesta_id");

            migrationBuilder.CreateIndex(
                name: "IX_composicao_cotacao_cotacao_id",
                table: "composicao_cotacao",
                column: "cotacao_id");

            migrationBuilder.CreateIndex(
                name: "IX_conta_grafica_cliente_id",
                table: "conta_grafica",
                column: "cliente_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_custodia_filhote_conta_grafica_id",
                table: "custodia_filhote",
                column: "conta_grafica_id");

            migrationBuilder.CreateIndex(
                name: "IX_custodia_master_conta_master_id",
                table: "custodia_master",
                column: "conta_master_id");

            migrationBuilder.CreateIndex(
                name: "IX_distribuicao_conta_grafica_id",
                table: "distribuicao",
                column: "conta_grafica_id");

            migrationBuilder.CreateIndex(
                name: "IX_distribuicao_ordem_compra_id",
                table: "distribuicao",
                column: "ordem_compra_id");

            migrationBuilder.CreateIndex(
                name: "IX_historico_compra_conta_grafica_id",
                table: "historico_compra",
                column: "conta_grafica_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordem_compra_detalhe_ordem_compra_id",
                table: "ordem_compra_detalhe",
                column: "ordem_compra_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "composicao_cesta");

            migrationBuilder.DropTable(
                name: "composicao_cotacao");

            migrationBuilder.DropTable(
                name: "custodia_filhote");

            migrationBuilder.DropTable(
                name: "custodia_master");

            migrationBuilder.DropTable(
                name: "distribuicao");

            migrationBuilder.DropTable(
                name: "historico_compra");

            migrationBuilder.DropTable(
                name: "historico_execucao_motor");

            migrationBuilder.DropTable(
                name: "ordem_compra_detalhe");

            migrationBuilder.DropTable(
                name: "cesta");

            migrationBuilder.DropTable(
                name: "cotacao");

            migrationBuilder.DropTable(
                name: "conta_master");

            migrationBuilder.DropTable(
                name: "conta_grafica");

            migrationBuilder.DropTable(
                name: "ordem_compra");

            migrationBuilder.DropTable(
                name: "cliente");
        }
    }
}
