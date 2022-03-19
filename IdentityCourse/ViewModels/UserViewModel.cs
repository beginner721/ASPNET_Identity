using System.ComponentModel.DataAnnotations;

namespace IdentityCourse.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Email adresi zorunludur.")]
        [Display(Name ="E-posta Adresi")]
        [EmailAddress(ErrorMessage ="Lütfen geçerli bir e-posta adresi girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        [Display(Name ="Şifre:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Kullanıcı adı girmek zorunludur.")]
        [Display(Name ="Kullanıcı Adı:")]
        public string UserName { get; set; }

        [Display(Name ="Telefon Numarası:")]
        public string PhoneNumber { get; set; }
    }
}
