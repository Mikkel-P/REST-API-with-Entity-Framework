using System.ComponentModel.DataAnnotations;

namespace API_ElevWiki.Models
{
    public class Student
    {
        // Sets studentId as primary key
        [Key]
        // Specifies that the field is required when inserting anything into the table
        [Required]
        public int studentId { get; set; }

        [Required]
        // Maxlength specifies how many chars that can be used
        [MaxLength(50)]
        public string name { get; set; }

        [Required]
        [MaxLength(50)]
        public string address { get; set; }

        [Required]
        [MaxLength(50)]
        public string phone { get; set; }
    }
}
