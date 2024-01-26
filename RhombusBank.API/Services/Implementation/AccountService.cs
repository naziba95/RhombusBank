using Microsoft.IdentityModel.Tokens;
using RhombusBank.API.Data;
using RhombusBank.API.Models;
using RhombusBank.API.Models.Domain;
using RhombusBank.API.Services.Interface;
using System.Text;

namespace RhombusBank.API.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _dbContext;

        public AccountService(AppDbContext dbContext)
        {
             _dbContext = dbContext;
        }

        public Account Authenticate(string AccountNumber, string Pin)
        {
            //First check if account number exists
            var account = _dbContext.Accounts.FirstOrDefault(x => x.AccountNumberGenerated == AccountNumber);
            if (account == null)
            {
                return null;
            }
            //verify pin hash
            if (!VerifyPinHash(Pin, account.PinHash, account.PinSalt)) return null;

            // If your code runs to this point, it means authentication was successfull.

            return account;

        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentException("Pin");
            // verify pin
            using(var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for(int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }

            return true;
        }



        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            // Create a new account
            // First things first, check if an account with the email provided already exists.
            if (_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("An account with this email already exists");
            {
                
            }
            // Hooray email doesn't exist, proceed to validate that pin and confirm pin match
            if (!Pin.Equals(ConfirmPin)) throw new ArgumentException("Pins do not match", "Pin");

            // All validations have passed, proceed to create account
            // First lets hash and encrypt pin

            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            // We are done with the Pin, now lets save account to db

            account.DateCreated = DateTime.UtcNow;
            account.DateLastUpdated= DateTime.UtcNow;
          

            _dbContext.Accounts.Add(account);   
            _dbContext.SaveChanges();

            return account;

        }

        // Create the CreatePinHash method
        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) 
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        public void Delete(int id)
        {
            var account = _dbContext.Accounts.FirstOrDefault(x => x.Id == id);
            if (account != null) 
            { 
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _dbContext.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _dbContext.Accounts.FirstOrDefault(x=>x.AccountNumberGenerated == AccountNumber);

            if (account == null) return null;

            return account;
        }

        public Account GetById(int id)
        {
            var account = _dbContext.Accounts.FirstOrDefault(x => x.Id == id);

            if (account == null) return null;

            return account;
        }

        public Account Update(Account account, string Pin = null)
        {
            var accountToBeUpdated = _dbContext.Accounts.FirstOrDefault(x=>x.Id == account.Id);
            if (accountToBeUpdated == null) throw new ApplicationException("Account does not exist");

            if(!string.IsNullOrWhiteSpace(account.Email)) //meaning email field contains an email and customer wants to update that too, first check if the new email doesnt already exist
            { 
                if (_dbContext.Accounts.Any(x=>x.Email==account.Email)) throw new ApplicationException($"This Email {account.Email} already exists");

                accountToBeUpdated.Email = account.Email;
                   
            }

            // The we are assuming Business rule allows customer to only update email, phone number and pin

            if (!string.IsNullOrWhiteSpace(account.PhoneNumber)) 
            {
                if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber)) throw new ApplicationException($"This Phone number {account.PhoneNumber} already exists");

                accountToBeUpdated.PhoneNumber = account.PhoneNumber;

            }


            if (!string.IsNullOrWhiteSpace(Pin))
            {
                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);

                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;
            }

            accountToBeUpdated.DateLastUpdated = DateTime.Now;


            _dbContext.Accounts.Update(accountToBeUpdated); 
            _dbContext.SaveChanges();

            return accountToBeUpdated;
        }
    }
}
