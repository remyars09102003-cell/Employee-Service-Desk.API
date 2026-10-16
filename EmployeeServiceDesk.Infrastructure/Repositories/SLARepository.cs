using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Enums;
using EmployeeServiceDesk.Domain.Interfaces;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeServiceDesk.Infrastructure.Repositories;

public class SLARepository : ISLARepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SLARepository> _logger;

    public SLARepository(ApplicationDbContext context,ILogger<SLARepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<SLA>> GetAllAsync()
    {
        try
        {
            return await _context.SLAs
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting all SLA rules");
            throw;
        }
    }

    public async Task<SLA?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.SLAs
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting SLA with Id {Id}", id);
            throw;
        }
    }

    public async Task<SLA?> GetByPriorityAsync(Priority priority)
    {
        try
        {
            return await _context.SLAs
                .FirstOrDefaultAsync(x =>
                    x.Priority == priority &&
                    x.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while getting SLA for priority {Priority}",
                priority);

            throw;
        }
    }

    public async Task<SLA> AddAsync(SLA sla)
    {
        try
        {
            await _context.SLAs.AddAsync(sla);
            await _context.SaveChangesAsync();

            return sla;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating SLA");
            throw;
        }
    }

    public async Task<SLA> UpdateAsync(SLA sla)
    {
        try
        {
            _context.SLAs.Update(sla);

            await _context.SaveChangesAsync();

            return sla;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while updating SLA with Id {Id}",sla.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var sla = await _context.SLAs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sla == null)
                return false;

            // Soft delete
            sla.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError( ex, "Error while deleting SLA with Id {Id}", id);
            throw;
        }
    }
}