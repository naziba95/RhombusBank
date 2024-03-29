﻿using RhombusBank.API.Models.Domain;

namespace RhombusBank.API.Models.DTO
{
    public class AccountDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CurrentAccountBalance { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeString => Enum.GetName(typeof(AccountType), AccountType); // Property to get AccountType name
        public string AccountNumberGenerated { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
    }
}
