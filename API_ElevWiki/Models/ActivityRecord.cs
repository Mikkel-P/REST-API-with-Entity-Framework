using System;
using System.ComponentModel.DataAnnotations;

namespace API_ElevWiki.Models
{
    public class ActivityRecord
    {
        [Key]
        [Required]
        public int activeId { get; set; }

        [Required]
        public bool activeStatus { get; set; }

        [Required]
        public DateTime timestamp { get; set; }

        [Required]
        public Student Student { get; set; }
    }
}
