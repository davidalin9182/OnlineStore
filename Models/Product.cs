using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Proiect_IR.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        // maybe name them something else
        public string? UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? ProductPrice{ get; set; }
        public string? ProductCategory { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? ProductImage { get; set; }
        public string? Sauces { get; set; }
        public int? Calories { get; set; }
        public int? Fat { get; set; }
        public int? Protein { get; set; }
        public double RelevanceScore { get; set; }




    }
}
