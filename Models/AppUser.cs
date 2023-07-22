using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Proiect_IR.Models
{
    public class AppUser : IdentityUser
    {

        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverUrl { get; set; }
        public ICollection<Product>? Products { get; set; }

    }
}
