using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingService.Models
{

    public class LikedDislikedVideoVote
    {
        [Key]
        [Column(Order = 0)]

        public int UserId { get; set; }
        [Key]
        [Column(Order = 1)]

        public int VideoId { get; set; }
        [Required]
        public bool Vote { get; set; }
    }
}
