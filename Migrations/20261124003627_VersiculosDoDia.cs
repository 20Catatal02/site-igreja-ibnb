using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IBNB.Migrations
{
    /// <inheritdoc />
    public partial class VersiculosDoDia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Imagem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Caminho = table.Column<string>(type: "text", nullable: false),
                    NomeOriginal = table.Column<string>(type: "text", nullable: true),
                    DataUpload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EventoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imagem_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Versiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Referencia = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    Traducao = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Autor = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    UltimaExibicao = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versiculos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VersiculosDoDia",
                columns: table => new
                {
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    VersiculoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersiculosDoDia", x => x.Data);
                    table.ForeignKey(
                        name: "FK_VersiculosDoDia_Versiculos_VersiculoId",
                        column: x => x.VersiculoId,
                        principalTable: "Versiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Imagem_EventoId",
                table: "Imagem",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Versiculos_Referencia_Traducao",
                table: "Versiculos",
                columns: new[] { "Referencia", "Traducao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VersiculosDoDia_VersiculoId",
                table: "VersiculosDoDia",
                column: "VersiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imagem");

            migrationBuilder.DropTable(
                name: "VersiculosDoDia");

            migrationBuilder.DropTable(
                name: "Versiculos");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);
        }
    }
}
