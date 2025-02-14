using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LynxAPI.Models;

public class Equipamento
{
        [Required]
        [MaxLength(10)]
        public string Instalacao { get; set; } = string.Empty;

        [Required]
        public int Lote { get; set; }

        [Required]
        [MaxLength(15)]
        public string Fabricante { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "VARCHAR(5)")]
        public OperadoraEnum Operadora { get; set; }

        [Required]
        public int Modelo { get; set; }

        [Required]
        public int Versao { get; set; }
}
public enum OperadoraEnum
{
        Claro,
        Tim,
        Vivo
}
