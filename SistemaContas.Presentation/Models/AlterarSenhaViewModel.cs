using System.ComponentModel.DataAnnotations;

namespace SistemaContas.Presentation.Models
{
    public class AlterarSenhaViewModel
    {
        [Required(ErrorMessage ="Por favor informe a nova senha.")]
        [MinLength(8, ErrorMessage = "Informe no mínimo {1} caracteres.")]
        [MaxLength(20, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string? NovaSenha { get; set; }

        [Required(ErrorMessage = "Por favor confirme a nova senha.")]
        [Compare("NovaSenha", ErrorMessage = "Senhas não conferem.")]
        public string? NovaSenhaConfirmacao { get; set; }
    }
}
