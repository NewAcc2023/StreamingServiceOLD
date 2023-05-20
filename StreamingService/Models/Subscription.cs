using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingService.Models
{

    public class Subscription
    {
        [Required]
        public int CreatorId { get; set; }
        [Required]
        public int FollowerId { get; set; }

        public User User1 { get; set; }
        public User User2 { get; set; }
    }
}
