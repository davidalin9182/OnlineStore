using Proiect_IR.Models;

namespace Proiect_IR.ViewModels
{
    public class CreateProductViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? ProductPrice { get; set; }
        public string? ProductCategory { get; set; }
        public IFormFile? ProductImage { get; set; }
        public string? Sauces { get; set; }
        public int? Calories { get; set; }
        public int? Fat { get; set; }
        public int? Protein { get; set; }
        public string? AppUserId { get; set; }


    }
}
