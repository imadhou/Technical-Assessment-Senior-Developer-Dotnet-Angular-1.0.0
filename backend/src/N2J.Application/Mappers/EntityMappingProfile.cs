using AutoMapper;
using N2J.Domain.Entities;
using N2J.Application.Models.Dtos;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos.Expense;
using N2J.Application.Models.Dtos.UserAuth;
using N2J.Application.Models.Dtos.Company;

namespace N2J.Application.Mappers;

/// <summary>
/// Provides mapping configurations between domain entities and their corresponding DTOs.
/// </summary>
public class EntityMappingProfile: Profile{

    public EntityMappingProfile()
    {
        CreateMap<Expense, ExpenseDto>();
        CreateMap<ExpenseCategory, ExpenseCategoryDto>();
        CreateMap<User, UserDto>();
        CreateMap<Company, CompanyDto>();
        CreateMap<ExpenseCommand, Expense>();
        CreateMap<Role, RoleDto>();
    }
}

