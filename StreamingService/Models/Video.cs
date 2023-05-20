using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingService.Models
{
    public class Video
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string VideoPath { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        [Required]
        [StringLength(5000)]
        public string Description { get; set; }
        [Required]
        public int CreatorId { get; set; }
        [Required]
        public int VideoViews { get; set; }
        [Required]
        public DateTime AddedAt { get; set; } = DateTime.Now;
        [Required]
        public string VideoThumbnailPath { get; set; }

        public User User { get; set; }
    }
}
