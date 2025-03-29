using BankAPI.Data;
using BankAPI.Models.Audit;
using BankAPI.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Admin + "," + Role.Auditor)]
    public class AuditController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("account/{accountNumber}")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAccountAuditLogs(string accountNumber)
        {
            return await _context.AuditLogs
                .Where(a => a.TableName == "Account" && a.RecordId.Contains(accountNumber))
                .OrderByDescending(a => a.ChangedAt)
                .ToListAsync();
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogsByUser(string userId)
        {
            return await _context.AuditLogs
                .Where(a => a.ChangedBy == userId)
                .OrderByDescending(a => a.ChangedAt)
                .ToListAsync();
        }

        [HttpGet("by-date")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogsByDateRange(
            [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return await _context.AuditLogs
                .Where(a => a.ChangedAt >= fromDate && a.ChangedAt <= toDate)
                .OrderByDescending(a => a.ChangedAt)
                .ToListAsync();
        }
    }
}