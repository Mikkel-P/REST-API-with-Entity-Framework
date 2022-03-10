
using System;
using System.ComponentModel.DataAnnotations;

namespace API_ElevWiki.Models
{
    public class TimeRecord
    {
        // Sets recId as primary key
        [Key]
        // Specifies that the field is required when inserting anything into the table
        [Required]
        public int recId { get; set; }

        [Required]
        // Maxlength specifies how many chars that can be used
        [MaxLength(50)]
        public DateTime timestamp { get; set; }

        [Required]
        [MaxLength(1)]
        public string checkStatus { get; set; }

        // Reference properties from Student.cs
        [Required]
        // We get studentId as foreign key from the Student table        
        public Student Student { get; set; }
    }
}
