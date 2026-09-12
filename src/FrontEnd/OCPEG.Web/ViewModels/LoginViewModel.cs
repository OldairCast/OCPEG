using System.ComponentModel.DataAnnotations;

namespace OCPEG.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o Email")]
        [StringLength(20)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Informe a Senha")]
        [StringLength(20)]
        public string? Password { get; set; }
    }
}
