using System.ComponentModel.DataAnnotations;

namespace RhombusBank.API.Models.DTO
{
    public class AuthenticateUserDto
    {
        [Required]
        [RegularExpression(@"[0][1-9]\d{9}$|^[1-9]\d{9}$")]
        public string AccountNumber { get; set; }
        public string Pin { get; set; }
    }
}
