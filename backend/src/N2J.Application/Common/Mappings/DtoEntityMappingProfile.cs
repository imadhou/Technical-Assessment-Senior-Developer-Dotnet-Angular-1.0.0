using AutoMapper;
using N2J.Domain.Entities;
using N2J.Application.Models.Dtos;

namespace N2J.Application.Common.Mappings;

public class DtoEntityMappingProfile: Profile{

    public DtoEntityMappingProfile()
    {
        // Expense mappings
        CreateMap<Expense, ExpenseDto>();
        CreateMap<ExpenseCategory, ExpenseCategoryDto>();
        CreateMap<User, UserDto>();
    }
}

