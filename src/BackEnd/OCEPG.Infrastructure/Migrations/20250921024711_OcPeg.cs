using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCEPG.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OcPeg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assunto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assunto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoCliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Telefone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true),
                    Administrador = table.Column<bool>(type: "bit", nullable: true),
                    Funcao = table.Column<short>(type: "smallint", nullable: true),
                    RefreshToken = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    //UserIdentifier = table.Column<Guid>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRole",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRole", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Telefone = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Idade = table.Column<short>(type: "smallint", nullable: true),
                    DataInscricao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Foto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Texto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Mensalidade = table.Column<decimal>(type: "smallmoney", nullable: true),
                    TipoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cliente_Tipo",
                        column: x => x.TipoId,
                        principalTable: "TipoCliente",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Atendimento",
                columns: table => new
                {
                    Protocolo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comentario = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    IdCliente = table.Column<int>(type: "int", nullable: true),
                    IdAssunto = table.Column<int>(type: "int", nullable: true),
                    Prioridade = table.Column<byte>(type: "tinyint", nullable: true),
                    StatusAtend = table.Column<byte>(type: "tinyint", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atendimento", x => x.Protocolo);
                    table.ForeignKey(
                        name: "FK_Atendimento_Assunto",
                        column: x => x.IdAssunto,
                        principalTable: "Assunto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Atendimento_Cliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Atendimento_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Resolucao",
                columns: table => new
                {
                    Protocolo = table.Column<int>(type: "int", nullable: false),
                    DataSolucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comentario = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resolucao", x => x.Protocolo);
                    table.ForeignKey(
                        name: "FK_Resolucao_Resolucao",
                        column: x => x.Protocolo,
                        principalTable: "Atendimento",
                        principalColumn: "Protocolo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atendimento_IdAssunto",
                table: "Atendimento",
                column: "IdAssunto");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimento_IdCliente",
                table: "Atendimento",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimento_UsuarioId",
                table: "Atendimento",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_TipoId",
                table: "Cliente",
                column: "TipoId");

            migrationBuilder.Sql("ALTER TABLE Usuario ADD UserIdentifier uniqueidentifier ROWGUIDCOL DEFAULT NEWID()");
            migrationBuilder.Sql("INSERT INTO Usuario (Id,Nome,Email,Telefone,PasswordHash,Active,Administrador,Funcao,RefreshToken,RefreshTokenExpiryTime)\r\nValues (1,'Usuario OcPeg','ocpeg@ocpeg.com','11988880999','f28c56dd3b29e2c1c90ec3183f65185067bd789082c80612eb79bd74088a9fe9',1,1,1,1,getdate())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resolucao");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "UsuarioRole");

            migrationBuilder.DropTable(
                name: "Atendimento");

            migrationBuilder.DropTable(
                name: "Assunto");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "TipoCliente");
        }
    }
}
