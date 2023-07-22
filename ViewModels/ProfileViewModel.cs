using Proiect_IR.Models;

namespace Proiect_IR.ViewModels
{
    public class ProfileViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Address { get; set; }
        public List<Product>? Products { get; set; }
    }
}
