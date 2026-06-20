using AutoMapper;
using FinanceControl.Application.DTOs.Transaction;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Application.Mappings;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<CreateTransactionDto, Transaction>();
        CreateMap<UpdateTransactionDto, Transaction>();

        CreateMap<Transaction, TransactionResponseDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null
                    ? src.Category.Name
                    : string.Empty))
            .ForMember(dest => dest.CategoryColor,
                opt => opt.MapFrom(src => src.Category != null
                    ? src.Category.Color
                    : string.Empty));
    }
}