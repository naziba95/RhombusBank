using AutoMapper;
using RhombusBank.API.Models.Domain;
using RhombusBank.API.Models.DTO;

namespace RhombusBank.API.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterAccountDto, Account>().ReverseMap();
            CreateMap<UpdateAccountDto, Account>();
            CreateMap<Account, UpdateAccountDto>();
            CreateMap<Account, GetAccountDetailsDto>().ReverseMap();
            CreateMap<GetAccountDetailsDto, Account>();
            CreateMap<AccountDto, Account>();
            CreateMap<Account, AccountDto>();
            CreateMap<Transaction, TransactionDto>();
            CreateMap<TransactionDto, Transaction>();
        }
    }
}
