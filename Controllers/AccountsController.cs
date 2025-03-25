using BankAPI.Data;
using BankAPI.DTOs;
using BankAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")] // Base route: /api/accounts
    [ApiController]
    [Authorize] // Requires authentication for all endpoints
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor with dependency injection
        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Accounts/Holder/{name}
       [HttpGet("Holder/{name}")]
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

        // GET: api/Accounts/{accountNumber}
[HttpGet("{accountNumber}")]
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