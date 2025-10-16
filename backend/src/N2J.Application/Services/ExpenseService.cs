using AutoMapper;
using Microsoft.Extensions.Logging;
using N2J.Application.Configs;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos;
using N2J.Application.Models.Exceptions;
using N2J.Application.Models.Queries;
using N2J.Domain.Entities;
using N2J.Domain.Interfaces;
using System.Linq.Expressions;

namespace N2J.Application.Services;

public class ExpenseService: IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;
    private readonly IExpenceCategoryRepository _expenceCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRedisCacheService _cacheService;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(
        IExpenseRepository expenseRepository, 
        IMapper mapper, 
        IExpenceCategoryRepository expenceCategoryRepository, 
        IUserRepository userRepository,
        IRedisCacheService cacheService,
        ILogger<ExpenseService> logger,
        IFileStorageService fileStorageService)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
        _expenceCategoryRepository = expenceCategoryRepository;
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
        _cacheService = cacheService;
        _logger = logger;
    }


    public async Task DeleteExpenseAsync(Guid expenseId, Guid companyId)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId, e => e.User);

        if (expense == null)
        {
            throw new EntityNotFoundException($"Expense not found.");
        }

        if (expense.User.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException();
        }
        string? expenseReceiptUrl = expense.ReceiptUrl;
        await _expenseRepository.DeleteAsync(expense, logChanges: true);
        if (!string.IsNullOrWhiteSpace(expenseReceiptUrl))
        {
            await _fileStorageService.DeleteFileAsync(expenseReceiptUrl);
        }
        await _cacheService.RemoveAsync(CacheKeys.ExpenseById(expenseId));
        await InvalidateCompanyCaches(companyId);
    }
    public async Task<ExpenseDto> UpdateExpenseStatusAsync(Guid expenseId, string newStatus, Guid updatedBy, Guid companyId)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId, e => e.User);
        if (expense == null)
        {
            throw new EntityNotFoundException("Expnese not found");
        }

        if(expense.User.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException();
        }

        expense.Status = newStatus;
        expense.UpdatedAt = DateTime.UtcNow;
        expense.UpdatedBy = updatedBy;

        await _expenseRepository.UpdateAsync(expense, updatedBy, logChanges: true);

        // Invalidate caches
        await _cacheService.RemoveAsync(CacheKeys.ExpenseById(expenseId));
        await InvalidateCompanyCaches(expense.User.CompanyId);
        return _mapper.Map<ExpenseDto>(expense);
    }

    public async Task<ExpenseDto> UpdateExpenseAsync(Guid expenseId, ExpenseCommand expenseCommand, Guid updatedBy, Guid companyId)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId, e => e.User);

        if (expense == null)
        {
            throw new EntityNotFoundException($"Expense not found.");
        }

        if (expense.User.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException();
        }

        var expenseCategory = await _expenceCategoryRepository.GetByIdAsync(expenseCommand.ExpenseCategoryId);
        if (expenseCategory == null)
        {
            throw new ReferenceNotFoundException($"Expense category with ID {expenseCommand.ExpenseCategoryId} not found.");
        }

        var expenseNewUser = await _userRepository.GetByIdAsync(expenseCommand.UserId);
        if (expenseNewUser == null)
        {
            throw new ReferenceNotFoundException($"User with ID {expenseCommand.UserId} not found.");
        }

        if (expenseNewUser.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException();
        }

        expense = _mapper.Map(expenseCommand, expense);
        expense.UpdatedAt = DateTime.UtcNow;
        expense.UpdatedBy = updatedBy;

        await _expenseRepository.UpdateAsync(expense, updatedBy, logChanges: true);

        // Invalidate caches
        await _cacheService.RemoveAsync(CacheKeys.ExpenseById(expenseId));
        await InvalidateCompanyCaches(expense.User.CompanyId);

        return _mapper.Map<ExpenseDto>(expense);
    }

    public async Task<ExpenseDto> CreateExpenseAsync(ExpenseCommand expenseCommand, Guid createdBy, Guid companyId)
    {
        var expenseCategory = await _expenceCategoryRepository.GetByIdAsync(expenseCommand.ExpenseCategoryId);
        if (expenseCategory == null)
        {
            throw new ReferenceNotFoundException($"Expense category with ID {expenseCommand.ExpenseCategoryId} not found.");
        }

        var expenseUser = await _userRepository.GetByIdAsync(expenseCommand.UserId);
        if (expenseUser == null)
        {
            throw new ReferenceNotFoundException($"User with ID {expenseCommand.UserId} not found.");
        }

        if(expenseUser.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException();
        }

        expenseCommand.UserId = expenseCommand.UserId;
        Expense expense = _mapper.Map<Expense>(expenseCommand);
        expense.CreatedBy = createdBy;
        expense.CreatedAt = DateTime.UtcNow;

        await _expenseRepository.AddAsync(expense, createdBy, logChanges: true);

        await InvalidateCompanyCaches(companyId);

        return _mapper.Map<ExpenseDto>(expense);
    }



    public async Task<ICollection<ExpenseDto>> GetAllAsync(ExpenseQuery query, Guid companyId)
    {
        var predicates = BuildPredicates(query, companyId);
        var orderBy = GetOrderByExpression(query);

        var expenses = await _expenseRepository.GetAllAsync(
            predicates: predicates,
            orderBy: orderBy,
            take: query.PageSize,
            skip: (query.Page - 1) * query.PageSize,
            e => e.ExpenseCategory,
            e => e.User
        );
        var expenseDtos = _mapper.Map<ICollection<ExpenseDto>>(expenses);

        return expenseDtos;
    }

    public async Task<(ICollection<ExpenseDto> Items, int TotalCount)> GetAllWithCountAsync(ExpenseQuery query, Guid companyId)
    {
        var cacheKey = BuildCacheKey(query, companyId);
        var groupKey = GetCompanyGroupKey(companyId);

        // Try to get from cache
        var cachedResult = await _cacheService.GetAsync<(ICollection<ExpenseDto> Items, int TotalCount)>(cacheKey);
        if (cachedResult.Items != null)
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
            return cachedResult;
        }

        var predicates = BuildPredicates(query, companyId);
        var orderBy = GetOrderByExpression(query);

        var expenses = await _expenseRepository.GetAllWithCountAsync(
            predicates: predicates,
            orderBy: orderBy,
            isDescending: query.IsDescending,
            take: query.PageSize,
            skip: (query.Page - 1) * query.PageSize,
            e => e.ExpenseCategory,
            e => e.User
        );
        var expenseDtos = _mapper.Map<ICollection<ExpenseDto>>(expenses.Items);
        var result = (expenseDtos, expenses.TotalCount);

        // Cache the result and track the key
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));
        await _cacheService.AddKeyToGroupAsync(groupKey, cacheKey);

        return result;
    }

    public async Task<ExpenseDto?> GetByIdAsync(Guid id)
    { 
        var expense = await _expenseRepository.GetByIdAsync(id, e => e.ExpenseCategory, e => e.User);
        if (expense == null) return null;
        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return expenseDto;
    }

    private ICollection<Expression<Func<Expense, bool>>> BuildPredicates(ExpenseQuery query, Guid companyId)
    {
        var predicates = new List<Expression<Func<Expense, bool>>>();
        predicates.Add(e => e.User.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            predicates.Add(e => e.Description.Contains(query.Description));
        }

        if (query.ExpenseCategoryId.HasValue)
        {
            predicates.Add(e => e.ExpenseCategoryId == query.ExpenseCategoryId);
        }

        if (query.MinAmount.HasValue)
        {
            predicates.Add(e => e.Amount >= query.MinAmount.Value);
        }

        if (query.MaxAmount.HasValue)
        {
            predicates.Add(e => e.Amount <= query.MaxAmount.Value);
        }

        if (query.FromDate.HasValue)
        {
            predicates.Add(e => e.Date >= query.FromDate.Value.Date);
        }

        if (query.ToDate.HasValue)
        {
            predicates.Add(e => e.Date <= query.ToDate.Value.Date);
        }

        if (query.UserId.HasValue)
        {
            predicates.Add(e => e.UserId == query.UserId);
        }


        return predicates;
    }

    private Expression<Func<Expense, object>> GetOrderByExpression(ExpenseQuery query)
    {
        return query.SortBy?.ToLower() switch
        {
            "amount" => e => e.Amount,
            "date" => e => e.Date,
            "description" => e => e.Description,
            "expenseCategoryId" => e => e.ExpenseCategoryId,
            "createdat" => e => e.CreatedAt,
            _ => e => e.CreatedAt // Default
        };
    }


    private string BuildCacheKey(ExpenseQuery query, Guid companyId)
    {
        // Determine which cache key pattern to use based on query parameters
        if (query.ExpenseCategoryId.HasValue && !string.IsNullOrEmpty(query.Status))
        {
            return CacheKeys.ExpensesByCompanyAndCategory(
                companyId, query.ExpenseCategoryId.Value, query.Page, query.PageSize,
                query.SortBy ?? "createdat", query.IsDescending);
        }
        else if (!string.IsNullOrEmpty(query.Status))
        {
            return CacheKeys.ExpensesByCompanyAndStatus(
                companyId, query.Status, query.Page, query.PageSize,
                query.SortBy ?? "createdat", query.IsDescending);
        }
        else
        {
            return CacheKeys.ExpensesByCompany(
                companyId, query.Page, query.PageSize,
                query.SortBy ?? "createdat", query.IsDescending);
        }
    }

    private string GetCompanyGroupKey(Guid companyId) => $"company:{companyId}";

    private async Task InvalidateCompanyCaches(Guid companyId)
    {
        var groupKey = GetCompanyGroupKey(companyId);
        await _cacheService.RemoveGroupAsync(groupKey);
        _logger.LogInformation("Invalidated all caches for company {CompanyId}", companyId);
    }
}
