using System.ComponentModel.DataAnnotations;

namespace API_ElevWiki.Models
{
    public class NfcCard
    {
        // Sets nfcId as primary key
        [Key]
        // Specifies that the field is required when inserting anything into the table
        [Required]
        public int nfcId { get; set; }

        [Required]
        // Maxlength specifies how many chars that can be used
        [MaxLength(12)]
        public string nfcTag { get; set; }

        // References properties from Student.cs
        [Required]
        // We get studentId as foreign key from the Student table
        public Student Student { get; set; }
    }
}
