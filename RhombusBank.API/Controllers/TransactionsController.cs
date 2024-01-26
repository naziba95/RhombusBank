using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RhombusBank.API.Models.DTO;
using RhombusBank.API.Services.Interface;
using System.Text.RegularExpressions;

namespace RhombusBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        public TransactionsController(IMapper mapper, ITransactionService transactionService)
        {
            _mapper = mapper;
            _transactionService = transactionService;  
        }

        [HttpGet]
        [Route("Get_Transaction_By_AccountNumber")]
        public IActionResult GetTransactionByAccount(string AccountNumber)
        {
            if (!Regex.IsMatch(AccountNumber, @"[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account number must be 10 digits");
            
           var accountDetails = _transactionService.GetTransactionByAccount(AccountNumber);

            return Ok(accountDetails);
            
        }


        [HttpGet]
        [Route("Get_Transaction_Details_ByReference")]
        public IActionResult GetTransactionByReferenceId(string TransactionRef)
        {
            //if (!Regex.IsMatch(TransactionRef, @"^\d{27}$")) return BadRequest("Transaction reference must be 27 digits");

            var accountDetails = _transactionService.GetTransactionByReferenceId(TransactionRef);

            return Ok(accountDetails);

        }


        // Implement Get transaction by DateTime here
        [HttpGet]
        [Route("Get_Transaction_ByDate")]
        public IActionResult FindTransactionByDate(DateTime Date)
        {
            var account = _transactionService.FindTransactionByDate(Date); 
            
            return Ok(account);
        }

        [HttpGet]
        [Route("Get_All_Transactions")]
        public IActionResult GetAllTransactions()
        {
            var transaction = _transactionService.GetAllTransactions();

            return Ok(_mapper.Map<List<TransactionDto>>(transaction));
        }


        [HttpPost]
        [Route("Deposit_Funds")]
        public IActionResult MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            var transaction = _transactionService.MakeDeposit(AccountNumber, Amount, TransactionPin);

            return Ok(transaction);

        }

        [HttpPost]
        [Route("Transfer_Funds")]
        public IActionResult MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            var transaction = _transactionService.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin);

            return Ok(transaction);

        }

        [HttpPost]
        [Route("Withdraw_Funds")]
        public IActionResult MakeWithDrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            var transaction = _transactionService.MakeWithDrawal(AccountNumber, Amount, TransactionPin);

            return Ok(transaction);

        }


    }
}
