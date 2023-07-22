
namespace Proiect_IR.ViewModels
{
    public class EditProfileViewModel
    {
        public string Id { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? UserName { get;set; }
        public string? Address { get; set; }
        public IFormFile? Image { get; set; }


    }
}