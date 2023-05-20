using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingService.Models
{

    public class LikedDislikedCommentVote
    {
        [Required]

        public int UserId { get; set; }
        [Required]

        public int CommentId { get; set; }
        [Required]
        public bool Vote { get; set; }
    }
}
