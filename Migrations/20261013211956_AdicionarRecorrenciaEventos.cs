using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBNB.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRecorrenciaEventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiaDaSemana",
                table: "Eventos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IndiceSemana",
                table: "Eventos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecorrente",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TipoRecorrencia",
                table: "Eventos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaDaSemana",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "IndiceSemana",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "IsRecorrente",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "TipoRecorrencia",
                table: "Eventos");
        }
    }
}
