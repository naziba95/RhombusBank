using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using System.Text.Json.Serialization;

namespace RhombusBank.API.Models.Domain
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CurrentAccountBalance { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeString => Enum.GetName(typeof(AccountType), AccountType); // Property to get AccountType name
        public string AccountNumberGenerated { get; set; }

        // Store hash and salt of transaction pin
        [JsonIgnore]
        public byte[] PinHash { get; set; }
        [JsonIgnore]
        public byte[] PinSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }

        // Add logic to generate account number in the constructor

        Random rand = new Random();


        public Account()
        {
            AccountName = $"{FirstName} {LastName}";

            AccountNumberGenerated = Convert.ToString((long)Math.Floor(rand.NextDouble() 
                * (9_000_000_000L - 1_000_000_000L + 1) + 1_000_000_000L));
            // Account name is a combination of Firstname and lastname
            
        }

    }


    public enum AccountType
    {
        Savings,
        Current,
        Corporate,
        Government,
        Office
    }
}
