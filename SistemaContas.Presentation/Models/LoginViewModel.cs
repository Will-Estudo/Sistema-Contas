using System.ComponentModel.DataAnnotations;

namespace SistemaContas.Presentation.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Por favor informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Por favor informe sua senha.")]
        [MinLength(8, ErrorMessage = "Informe no mínimo {1} caracteres.")]
        [MaxLength(20, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string? Senha { get; set; }
    }
}
