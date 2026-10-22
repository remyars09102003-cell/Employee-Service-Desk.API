using EmployeeServiceDesk.Application.DTOs.SLA;
using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.Application.Interfaces;

public interface ISLAService
{
    Task<List<SlaDto>> GetAllAsync();

    Task<SlaDto?> GetByIdAsync(int id);

    Task<SlaDto> CreateAsync(CreateSlaDto dto);

    Task<SlaDto?> UpdateAsync(int id, UpdateSlaDto dto);

    Task<bool> DeleteAsync(int id);

    Task<(DateTime ResponseDeadline, DateTime ResolutionDeadline)> CalculateDeadlineAsync(Priority priority);
}