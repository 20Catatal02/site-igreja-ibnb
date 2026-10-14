using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBNB.Migrations
{
    /// <inheritdoc />
    public partial class RecorrenciaFlexivelEstiloXiaomi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FrequenciaPersonalizada",
                table: "Eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntervaloRecorrencia",
                table: "Eventos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrequenciaPersonalizada",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "IntervaloRecorrencia",
                table: "Eventos");
        }
    }
}
