using System.ComponentModel.DataAnnotations;

namespace SistemaContas.Presentation.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Por favor informe o nome do usuário")]
        [RegularExpression("^[A-Za-zÀ-Üà-ü\\s]{8,150}$", ErrorMessage = "Informe um nome válido de 8 a 150 caracteres.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "Por favor informe o e-mail do usuário")]
        [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Por favor informe a senha do usuário")]
        [MinLength(8, ErrorMessage = "Informe no mínimo {1} caracteres.")]
        [MaxLength(20, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "Por favor confirme a senha do usuário")]
        [Compare("Senha", ErrorMessage = "Senhas não conferem.")]
        public string? SenhaConfirmacao { get; set; }
    }
}
