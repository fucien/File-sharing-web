using System.ComponentModel.DataAnnotations;

namespace web_ver_2.Models
{
    public class File
    {
        [Key]
        [Required]
		public string URL { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        [Required]
		public string Email { get; set; } 
        public DateTime UploadedDate { get; set; } = DateTime.Now;
    }
}
