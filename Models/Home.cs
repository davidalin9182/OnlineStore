using System.ComponentModel.DataAnnotations;

namespace Proiect_IR.Models
{
   
    
        public class Home
        {
            [Key]
            public int Id { get; set; }
            public string? NewGames { get; set; }


        }
    
}
