using System.ComponentModel.DataAnnotations;

namespace IdentityCourse.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name ="E-posta Adresiniz")]
        [Required(ErrorMessage ="E-posta alanı boş bırakılamaz.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name ="Şifreniz")]
        [Required(ErrorMessage ="Şifre boş bırakılamaz.")]
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage ="Şifreniz en az 4 karakterli olmalıdır.")]
        public string Password { get; set; }

        public bool RememberMe { get; set;}
    }
}
