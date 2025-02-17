using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LynxAPI.Models;

public class Equipamento
{
        [Required]
        [MaxLength(10)]
        public string Instalacao { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Lote { get; set; }

        [Required]
        [MaxLength(15)]
        public string Fabricante { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "VARCHAR(5)")]
        public OperadoraEnum Operadora { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Modelo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Versao { get; set; }
}
public enum OperadoraEnum
{
        Claro,
        Tim,
        Vivo
}
