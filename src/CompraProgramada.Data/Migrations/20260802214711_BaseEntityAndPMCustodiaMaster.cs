using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntityAndPMCustodiaMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_conta_grafica_cliente_ClienteId",
                table: "conta_grafica");

            migrationBuilder.DropColumn(
                name: "data_adesao",
                table: "cliente");

            migrationBuilder.DropColumn(
                name: "data_desativacao",
                table: "cesta");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "conta_grafica",
                newName: "cliente_id");

            migrationBuilder.RenameIndex(
                name: "IX_conta_grafica_ClienteId",
                table: "conta_grafica",
                newName: "IX_conta_grafica_cliente_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_atualizacao",
                table: "custodia_master",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de atualização");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_criacao",
                table: "custodia_master",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de criação");

            migrationBuilder.AddColumn<decimal>(
                name: "preco_medio",
                table: "custodia_master",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "preço médio dos ativos");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_atualizacao",
                table: "custodia_filhote",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de atualização");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_criacao",
                table: "custodia_filhote",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de criação");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_atualizacao",
                table: "cotacao",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de atualização");

            migrationBuilder.AlterColumn<string>(
                name: "tipo",
                table: "conta_master",
                type: "longtext",
                nullable: false,
                comment: "tipo da conta",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "tipo conta")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "numero_conta",
                table: "conta_master",
                type: "longtext",
                nullable: false,
                comment: "número da conta",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "número conta")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "numero_conta",
                table: "conta_grafica",
                type: "longtext",
                nullable: false,
                comment: "número da conta",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "número conta")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "cliente_id",
                table: "conta_grafica",
                type: "int",
                nullable: false,
                comment: "identificador cliente",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_atualizacao",
                table: "cliente",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de atualização");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_criacao",
                table: "cliente",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de criação");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_criacao",
                table: "cesta",
                type: "datetime(6)",
                nullable: false,
                comment: "data de criação",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_atualizacao",
                table: "cesta",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de atualização");

            migrationBuilder.AddForeignKey(
                name: "FK_conta_grafica_cliente_cliente_id",
                table: "conta_grafica",
                column: "cliente_id",
                principalTable: "cliente",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_conta_grafica_cliente_cliente_id",
                table: "conta_grafica");

            migrationBuilder.DropColumn(
                name: "data_atualizacao",
                table: "custodia_master");

            migrationBuilder.DropColumn(
                name: "data_criacao",
                table: "custodia_master");

            migrationBuilder.DropColumn(
                name: "preco_medio",
                table: "custodia_master");

            migrationBuilder.DropColumn(
                name: "data_atualizacao",
                table: "custodia_filhote");

            migrationBuilder.DropColumn(
                name: "data_criacao",
                table: "custodia_filhote");

            migrationBuilder.DropColumn(
                name: "data_atualizacao",
                table: "cotacao");

            migrationBuilder.DropColumn(
                name: "data_atualizacao",
                table: "cliente");

            migrationBuilder.DropColumn(
                name: "data_criacao",
                table: "cliente");

            migrationBuilder.DropColumn(
                name: "data_atualizacao",
                table: "cesta");

            migrationBuilder.RenameColumn(
                name: "cliente_id",
                table: "conta_grafica",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_conta_grafica_cliente_id",
                table: "conta_grafica",
                newName: "IX_conta_grafica_ClienteId");

            migrationBuilder.AlterColumn<string>(
                name: "tipo",
                table: "conta_master",
                type: "longtext",
                nullable: false,
                comment: "tipo conta",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "tipo da conta")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "numero_conta",
                table: "conta_master",
                type: "longtext",
                nullable: false,
                comment: "número conta",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "número da conta")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "numero_conta",
                table: "conta_grafica",
                type: "longtext",
                nullable: false,
                comment: "número conta",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "número da conta")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "conta_grafica",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "identificador cliente");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_adesao",
                table: "cliente",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_criacao",
                table: "cesta",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "data de criação");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_desativacao",
                table: "cesta",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_conta_grafica_cliente_ClienteId",
                table: "conta_grafica",
                column: "ClienteId",
                principalTable: "cliente",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
