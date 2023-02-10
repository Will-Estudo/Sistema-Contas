using System.ComponentModel.DataAnnotations;

namespace SistemaContas.Presentation.Models
{
    public class PasswordRecoverViewModel
    {
        [Required(ErrorMessage = "Por favor informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido")]
        public string? Email { get; set; }
    }
}
