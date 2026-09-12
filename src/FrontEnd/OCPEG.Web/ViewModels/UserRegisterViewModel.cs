using System.ComponentModel.DataAnnotations;

namespace OCPEG.Web.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Informe o Nome")]
        [StringLength(256)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Informe o Sobrenome")]
        [StringLength(256)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Informe o login do Usuário")]
        [StringLength(30)]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Informe o Email")]
        [StringLength(256)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Informe o Telefone")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Informe a Senha")]
        [StringLength(20)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Redigite a Senha")]
        [StringLength(20)]
        public string? RetypePassword { get; set; }

    }
}
