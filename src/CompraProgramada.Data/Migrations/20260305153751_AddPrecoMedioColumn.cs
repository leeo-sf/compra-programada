using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecoMedioColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "preco_medio",
                table: "custodia_filhote",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "custo médio ponderado de compra");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "preco_medio",
                table: "custodia_filhote");
        }
    }
}
