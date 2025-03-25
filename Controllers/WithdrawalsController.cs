using BankAPI.Data;
using BankAPI.DTOs;
using BankAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")] // Base route: /api/withdrawals
    [ApiController]
    [Authorize] // Requires authentication
    public class WithdrawalsController : ControllerBase
    {
         private readonly ApplicationDbContext _context;
        private readonly ILogger<WithdrawalsController> _logger;

        public WithdrawalsController(
            ApplicationDbContext context, 
            ILogger<WithdrawalsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/Withdrawals
        [HttpPost]
        public async Task<ActionResult<WithdrawalDTO>> CreateWithdrawal(
            CreateWithdrawalDTO createWithdrawalDTO)
        {
            try
            {
                // Find the account with tracking to update balance
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == createWithdrawalDTO.AccountNumber);

                if (account == null)
                {
                    _logger.LogWarning("Account not found: {AccountNumber}", 
                        createWithdrawalDTO.AccountNumber);
                    return NotFound(new { Message = "Account not found" });
                }

                // Validate withdrawal against business rules
                var validationResult = ValidateWithdrawal(account, createWithdrawalDTO.Amount);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Withdrawal validation failed: {ErrorMessage}", 
                        validationResult.ErrorMessage);
                    return BadRequest(new { Message = validationResult.ErrorMessage });
                }

                // Create new withdrawal
                var withdrawal = new Withdrawal
                {
                    AccountNumber = createWithdrawalDTO.AccountNumber,
                    Amount = createWithdrawalDTO.Amount,
                    TransactionDate = DateTime.UtcNow
                };

                // Update account balance
                account.AvailableBalance -= createWithdrawalDTO.Amount;

                // For fixed deposit accounts, close the account after full withdrawal
                if (account.Type == AccountType.FixedDeposit && 
                    account.AvailableBalance == 0)
                {
                    account.Status = AccountStatus.Closed;
                    _logger.LogInformation("Fixed deposit account {AccountNumber} closed after full withdrawal", 
                        account.AccountNumber);
                }

                _context.Withdrawals.Add(withdrawal);
                await _context.SaveChangesAsync();

                var withdrawalDTO = new WithdrawalDTO
                {
                    Id = withdrawal.Id,
                    AccountNumber = withdrawal.AccountNumber,
                    TransactionDate = withdrawal.TransactionDate,
                    Amount = withdrawal.Amount
                };

                _logger.LogInformation("Withdrawal created successfully for account {AccountNumber}", 
                    withdrawal.AccountNumber);
                
                return CreatedAtAction(
                    nameof(CreateWithdrawal), 
                    new { id = withdrawal.Id }, 
                    withdrawalDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal for account {AccountNumber}", 
                    createWithdrawalDTO.AccountNumber);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        private ValidationResult ValidateWithdrawal(Account account, decimal amount)
        {
            // Validate amount is positive
            if (amount <= 0)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Withdrawal amount must be greater than 0"
                };
            }

            // Validate account status
            if (account.Status != AccountStatus.Active)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Withdrawals are not allowed on {account.Status.ToString().ToLower()} accounts"
                };
            }

            // Validate sufficient balance
            if (amount > account.AvailableBalance)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Withdrawal amount exceeds available balance"
                };
            }

            // Validate fixed deposit rules
            if (account.Type == AccountType.FixedDeposit)
            {
                if (amount != account.AvailableBalance)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Only full (100%) withdrawals are allowed on fixed deposit accounts"
                    };
                }
            }

            return new ValidationResult { IsValid = true };
        }
    }

    // Helper class for validation results
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}