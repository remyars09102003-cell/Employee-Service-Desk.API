using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.Domain.Interfaces;

public interface ISLARepository
{
    Task<List<SLA>> GetAllAsync();

    Task<SLA?> GetByIdAsync(int id);

    Task<SLA?> GetByPriorityAsync(Priority priority);

    Task<SLA> AddAsync(SLA sla);

    Task<SLA> UpdateAsync(SLA sla);

    Task<bool> DeleteAsync(int id);
}