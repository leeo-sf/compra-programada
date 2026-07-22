using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class DataCriacaoSchemaCotacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "data_pregao",
                table: "cotacao",
                type: "date",
                nullable: false,
                comment: "data do pregão",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "data do pregão");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_criacao",
                table: "cotacao",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "data de criação");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_criacao",
                table: "cotacao");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_pregao",
                table: "cotacao",
                type: "datetime(6)",
                nullable: false,
                comment: "data do pregão",
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldComment: "data do pregão");
        }
    }
}
