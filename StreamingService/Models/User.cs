using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StreamingService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(255)]
        public string LastName { get; set; }
        [Required]
        [StringLength(255)]
        public string Nickname { get; set; }
        [Required]
        public string ImagePath { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText(true)]
        public string UserPassword { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }

        public List<Video> Videos { get; set; }
        public List<Subscription> s1 { get; set; }
        public List<Subscription> s2 { get; set; }
    }
}
