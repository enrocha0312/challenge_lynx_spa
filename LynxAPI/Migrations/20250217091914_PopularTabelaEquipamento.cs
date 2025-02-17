using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LynxAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopularTabelaEquipamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Equipamentos (Instalacao, Lote, Fabricante, Operadora, Modelo, Versao) 
                VALUES ('InstA', 1, 'FabricanteX', 'Claro', 100, 1);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Equipamentos (Instalacao, Lote, Fabricante, Operadora, Modelo, Versao) 
                VALUES ('InstB', 2, 'FabricanteY', 'Tim', 200, 2);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Equipamentos (Instalacao, Lote, Fabricante, Operadora, Modelo, Versao) 
                VALUES ('InstC', 3, 'FabricanteZ', 'Vivo', 300, 3);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Equipamentos WHERE Instalacao IN ('InstA', 'InstB', 'InstC');");
        }
    }
}
