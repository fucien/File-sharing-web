using System.ComponentModel.DataAnnotations;

namespace web_ver_2.Models
{
    public class User
    {
        [Key]
        [Required]
		public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        
    }
}
