using EmployeeServiceDesk.Application.DTOs.Employees;

namespace EmployeeServiceDesk.Application.ServiceInterface
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<EmployeeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<EmployeeDto> CreateAsync(
            CreateEmployeeDto dto,
            CancellationToken cancellationToken = default);

        Task<EmployeeDto?> UpdateAsync(
            int id,
            UpdateEmployeeDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> DeactivateAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}