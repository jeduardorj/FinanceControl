using AutoMapper;
using FinanceControl.Application.DTOs.Transaction;
using FinanceControl.Application.Interfaces;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enums;
using FinanceControl.Domain.Exceptions;
using FinanceControl.Domain.Interfaces;

namespace FinanceControl.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TransactionResponseDto> CreateAsync(
        Guid userId, CreateTransactionDto dto)
    {
        // Valida se a categoria pertence ao usuário
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);

        if (category is null || category.UserId != userId)
            throw new NotFoundException("Categoria", dto.CategoryId);

        var transaction = _mapper.Map<Transaction>(dto);
        transaction.UserId = userId;

        await _unitOfWork.Transactions.AddAsync(transaction);
        await _unitOfWork.CommitAsync();

        // Recarrega com a categoria para o mapeamento
        transaction.Category = category;

        return _mapper.Map<TransactionResponseDto>(transaction);
    }

    public async Task<IEnumerable<TransactionResponseDto>> GetAllByUserIdAsync(
        Guid userId, TransactionType? type = null)
    {
        var transactions = await _unitOfWork.Transactions.GetByUserIdAsync(userId);

        if (type.HasValue)
            transactions = transactions.Where(t => t.Type == type.Value);

        return _mapper.Map<IEnumerable<TransactionResponseDto>>(transactions);
    }

    public async Task<TransactionResponseDto> GetByIdAsync(
        Guid userId, Guid transactionId)
    {
        var transaction = await GetTransactionAndValidateOwnership(userId, transactionId);
        return _mapper.Map<TransactionResponseDto>(transaction);
    }

    public async Task<TransactionResponseDto> UpdateAsync(
        Guid userId, Guid transactionId, UpdateTransactionDto dto)
    {
        var transaction = await GetTransactionAndValidateOwnership(userId, transactionId);

        // Valida se a nova categoria pertence ao usuário
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);

        if (category is null || category.UserId != userId)
            throw new NotFoundException("Categoria", dto.CategoryId);

        transaction.Description = dto.Description;
        transaction.Amount = dto.Amount;
        transaction.Date = dto.Date;
        transaction.Type = dto.Type;
        transaction.CategoryId = dto.CategoryId;
        transaction.Category = category;

        _unitOfWork.Transactions.Update(transaction);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<TransactionResponseDto>(transaction);
    }

    public async Task DeleteAsync(Guid userId, Guid transactionId)
    {
        var transaction = await GetTransactionAndValidateOwnership(userId, transactionId);

        _unitOfWork.Transactions.Delete(transaction);
        await _unitOfWork.CommitAsync();
    }

    private async Task<Transaction> GetTransactionAndValidateOwnership(
        Guid userId, Guid transactionId)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);

        if (transaction is null || transaction.UserId != userId)
            throw new NotFoundException("Transação", transactionId);

        return transaction;
    }
}