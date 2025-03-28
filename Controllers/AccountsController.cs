using BankAPI.Data;
using BankAPI.DTOs;
using BankAPI.Models;
using BankAPI.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")] // Base route: /api/accounts
    [ApiController]
    [Authorize(Roles = Role.Admin + "," + Role.Banker + "," + Role.Customer)]//Added role-based authorization
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor with dependency injection
        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }
 /// <summary>
    /// Retrieves all accounts for a specific account holder
    /// </summary>
    /// <param name="name">The account holder's full name (firstname lastname)</param>
    /// <returns>List of accounts for the specified holder</returns>
    /// <response code="200">Returns the list of accounts</response>
    /// <response code="404">If no accounts found for the holder</response>
        // GET: api/Accounts/Holder/{name}
       [HttpGet("Holder/{name}")]
       [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAccountsByHolder(string name)
{
    var accounts = await _context.Accounts
        .Include(a => a.AccountHolder) // Make sure to include AccountHolder
        .Where(a => a.AccountHolder.FirstName + " " + a.AccountHolder.LastName == name)
        .Select(a => new AccountDTO
        {
            AccountNumber = a.AccountNumber,
            Type = a.Type.ToString(),
            Status = a.Status.ToString(),
            AvailableBalance = a.AvailableBalance,
            AccountHolder = new AccountHolderDTO
            {
                FirstName = a.AccountHolder.FirstName,
                LastName = a.AccountHolder.LastName,
                IdNumber = a.AccountHolder.IdNumber,
                Email = a.AccountHolder.Email,
                MobileNumber = a.AccountHolder.MobileNumber
            }
        })
        .ToListAsync();

    if (!accounts.Any())
    {
        return NotFound("No accounts found for this holder");
    }

    return accounts;
}
        /// <summary>
        /// Retrieves all accounts (Admin and Banker only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Role.Admin + "," + Role.Banker)]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAllAccounts()
        {
            return await _context.Accounts
                .Include(a => a.AccountHolder)
                .Select(a => new AccountDTO
                {
                    AccountNumber = a.AccountNumber,
                    Type = a.Type.ToString(),
                    Status = a.Status.ToString(),
                    AvailableBalance = a.AvailableBalance,
                    AccountHolder = new AccountHolderDTO
                    {
                        FirstName = a.AccountHolder.FirstName,
                        LastName = a.AccountHolder.LastName
                    }
                })
                .ToListAsync();
        }

    /// <summary>
    /// Retrieves a specific bank account by account number
    /// </summary>
    /// <param name="accountNumber">The account number to retrieve</param>
    /// <returns>The requested account details</returns>
    /// <response code="200">Returns the requested account</response>
    /// <response code="404">If the account doesn't exist</response>
    [HttpGet("{accountNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<AccountDTO>> GetAccount(string accountNumber)
{
    var account = await _context.Accounts
        .Include(a => a.AccountHolder)
        .Where(a => a.AccountNumber == accountNumber)
        .Select(a => new AccountDTO
        {
            AccountNumber = a.AccountNumber,
            Type = a.Type.ToString(),
            Status = a.Status.ToString(),
            AvailableBalance = a.AvailableBalance,
            AccountHolder = new AccountHolderDTO
            {
                FirstName = a.AccountHolder.FirstName,
                LastName = a.AccountHolder.LastName,
                IdNumber = a.AccountHolder.IdNumber,
                Email = a.AccountHolder.Email,
                MobileNumber = a.AccountHolder.MobileNumber
            }
        })
        .FirstOrDefaultAsync();

    if (account == null)
    {
        return NotFound();
    }

    return account;
}
    }
}