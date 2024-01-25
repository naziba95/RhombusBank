using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using RhombusBank.API.Models.Domain;
using System.Runtime.InteropServices;

namespace RhombusBank.API.Services.Interface
{
    public interface IAccountService
    {
        Account Authenticate(string AccountNumber, string Pin);
        IEnumerable<Account> GetAllAccounts();
        Account Create(Account account, string Pin, string ConfirmPin);
        Account Update(Account account, string Pin=null);
        void Delete(int id);
        Account GetById(int id);
        Account GetByAccountNumber(string  AccountNumber);
    }
}
