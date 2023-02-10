using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SistemaContas.Presentation.Models
{
    public class ContasEdicaoViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Por favor, informe o nome da conta")]
        [MinLength(8, ErrorMessage = "Informe no mínimo {1} caracteres.")]
        [MaxLength(150, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "Por favor, informe o valor da conta")]
        public decimal? Valor { get; set; }

        [Required(ErrorMessage = "Por favor, informe a data da conta")]
        public string? Data { get; set; }

        [Required(ErrorMessage = "Por favor, informe o tipo da conta")]
        public int? Tipo { get; set; }

        [Required(ErrorMessage = "Por favor, informe a categoria da conta")]
        public Guid? IdCategoria { get; set; }

        [Required(ErrorMessage = "Por favor, informe as observações da conta")]
        public string? Observacoes { get; set; }

        /// <summary>
        /// Lista para exibir na página  as opções de categorias
        /// </summary>
        public List<SelectListItem>? Categorias { get; set; }
    }
}
