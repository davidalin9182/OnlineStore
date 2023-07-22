using System.ComponentModel.DataAnnotations;

namespace Proiect_IR.ViewModels
{
    public class RegisterViewModel
{
        //[Display(Name = "Username")]
        //[Required(ErrorMessage = "Username is required")]
        //public string Name { get; set; }
        //[Display(Name = "ProfileImage")]
        //[Required(ErrorMessage = "Profile Image is required")]
        //public string? ProfileImageUrl { get; set; }
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }
        //[Display(Name = "Country")]
        //[Required(ErrorMessage = "Your Country is required")]
        //public string Country { get; set; }
        //[Display(Name = "City")]
        //[Required(ErrorMessage = "Your City is required")]
        //public string City { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }
}
}
