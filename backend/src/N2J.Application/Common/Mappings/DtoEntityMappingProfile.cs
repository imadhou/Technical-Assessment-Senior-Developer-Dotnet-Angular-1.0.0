using AutoMapper;
using N2J.Domain.Entities;
using N2J.Application.Models.Dtos;

namespace N2J.Application.Common.Mappings;

/// <summary>
/// Provides mapping configurations between domain entities and their corresponding DTOs.
/// </summary>
public class DtoEntityMappingProfile: Profile{

    public DtoEntityMappingProfile()
    {
        CreateMap<Expense, ExpenseDto>();
        CreateMap<ExpenseCategory, ExpenseCategoryDto>();
        CreateMap<User, UserDto>();
    }
}

