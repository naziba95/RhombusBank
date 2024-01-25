using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RhombusBank.API.Models.Domain;
using RhombusBank.API.Models.DTO;
using RhombusBank.API.Services.Interface;
using System.Text.RegularExpressions;

namespace RhombusBank.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public AccountsController(IAccountService accountService, IMapper mapper)
        {
                _accountService = accountService;
                _mapper = mapper;
        }


        [HttpPost]
        [Route("Create_New_Customer")]
        public IActionResult RegisterNewAccount(RegisterAccountDto account)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newAccount = _mapper.Map<Account>(account);

            newAccount = _accountService.Create(newAccount, account.Pin, account.ConfirmPin);


            return Ok(_mapper.Map<AccountDto>(newAccount));


        }

        [HttpPost]
        [Route("Auth")]
        public IActionResult AuthenticateUser(AuthenticateUserDto authUser)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           return Ok(_accountService.Authenticate(authUser.AccountNumber, authUser.Pin));

            

        }

        [HttpGet]
        [Route("Get_All_Accounts")]
        public IActionResult GetAllAccounts()
        {

            var accounts = _accountService.GetAllAccounts();

            if(accounts == null)
            {
                return NotFound();
            }

          var cleanedAccounts =   _mapper.Map<IList<GetAccountDetailsDto>>(accounts);
          return Ok(cleanedAccounts); 
        }

        [HttpGet]
        [Route("Get_Account_By_AccountNumber")]
        public IActionResult GetByAccountNumber(string AccountNumber) 
        {
            if (!Regex.IsMatch(AccountNumber, @"[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account number must be 10 digits");

                var account = _accountService.GetByAccountNumber(AccountNumber);

            //if (account == null)
            //{
            //    return NotFound();
            //}

            var CleanedAccount = _mapper.Map<AccountDto>(account);

            return Ok(CleanedAccount);

        }

        [HttpGet]
        [Route("Get_Account_By_Id")]
        public IActionResult GetById(int id)
        {
            var account = _accountService.GetById(id);

            if (account == null)
            {
                return NotFound();
            }

            var CleanedAccount = _mapper.Map<AccountDto>(account);

            return Ok(CleanedAccount);

        }


        [HttpDelete]
        [Route("Delete_Account")]
        public IActionResult Delete(int id)
        {
            _accountService.Delete(id);

            return NoContent();

        }

        [HttpPost]
        [Route("Update_Account")]
        public IActionResult UpdateAccount(UpdateAccountDto updateAccount)
        {
            if (!ModelState.IsValid) return BadRequest(updateAccount);

            var account = _mapper.Map<Account>(updateAccount);

           var updatedAccount = _accountService.Update(account, updateAccount.Pin);

            return Ok(_mapper.Map<AccountDto>(updatedAccount));
        }
    }
}
