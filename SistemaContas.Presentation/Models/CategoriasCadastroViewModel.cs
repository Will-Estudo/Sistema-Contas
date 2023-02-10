using System.ComponentModel.DataAnnotations;

namespace SistemaContas.Presentation.Models
{
    public class CategoriasCadastroViewModel
    {
        [Required(ErrorMessage = "Por favor, informe o nome da categoria")]
        [MinLength(8, ErrorMessage = "Informe no mínimo {1} caracteres.")]
        [MaxLength(150, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string? Nome { get; set; }
    }
}
