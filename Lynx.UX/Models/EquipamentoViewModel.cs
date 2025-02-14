using System.ComponentModel.DataAnnotations;

namespace Lynx.UX.Models
{
    public class EquipamentoViewModel
    {
        [Required(ErrorMessage = "O campo Instalação é obrigatório.")]
        [MaxLength(10, ErrorMessage = "A Instalação deve ter no máximo 10 caracteres.")]
        public string Instalacao { get; set; }
        [Required(ErrorMessage = "O campo Fabricante é obrigatório.")]
        [MaxLength(15, ErrorMessage = "O Fabricante deve ter no máximo 15 caracteres.")]
        public string Fabricante { get; set; }
        [Required(ErrorMessage = "O campo Operadora é obrigatório.")]
        public OperadoraEnum Operadora { get; set; }
        [Required(ErrorMessage = "O campo Lote é obrigatório.")]
        [Range(1, 10, ErrorMessage = "O Lote deve estar entre 1 e 10.")]
        public int Lote { get; set; }
        [Required(ErrorMessage = "O campo Modelo é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O Modelo deve ser maior que 0.")]
        public int Modelo { get; set; }
        [Required(ErrorMessage = "O campo Versão é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "A Versão deve ser maior que 0.")]
        public int Versao { get; set; }
    }
}
