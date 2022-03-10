using System;
using System.ComponentModel.DataAnnotations;

namespace API_ElevWiki.Models
{
    public class LoginHolder
    {
        [Key]
        [Required]
        public int loginId { get; set; }

        [Required]
        [MaxLength(50)]
        public string email { get; set; }

        [Required]
        public string passwordHash { get; set; }

        [Required]
        public bool confirmedEmail { get; set; }

        [Required]
        public bool admin { get; set; }

        [Required]
        public string confirmationToken { get; set; }

        public string refreshToken { get; set; }

        public DateTime refreshTokenExpiryTime { get; set; }

        [Required]
        public Student Student { get; set; }
    }
}
