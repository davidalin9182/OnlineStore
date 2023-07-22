using System.ComponentModel.DataAnnotations;

namespace Proiect_IR.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Email { get; set; }
        
    }
}
