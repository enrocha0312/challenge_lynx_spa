using System.ComponentModel.DataAnnotations;

namespace LynxUX.Models;

public class EquipamentoViewModel
{
        public string Instalacao { get; set; }
        public string Fabricante { get; set; }
        public OperadoraEnum Operadora { get; set; }
        public int Lote { get; set; }
        public int Modelo { get; set; }
        public int Versao { get; set; }
}
