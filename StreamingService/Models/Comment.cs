using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingService.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public int UserId { get; set; }
        [Required]
        public int VideoId { get; set; }
        [Required]
        [StringLength(5000)]
        public string CommentText { get; set; }
        [Required]
        public DateTime AddedAt { get; set; } = DateTime.Now;

    }
}
