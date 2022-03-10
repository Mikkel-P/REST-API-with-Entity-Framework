using System;
using System.ComponentModel.DataAnnotations;

namespace API_ElevWiki.Models
{
    public class AbscenceRecord
    {
        [Key]
        [Required]
        public int abscenceId { get; set; }

        [Required]
        public bool legalAbscence { get; set; }

        [Required]
        public DateTime timestamp { get; set; }

        [Required]
        public Student Student { get; set; }
    }
}
