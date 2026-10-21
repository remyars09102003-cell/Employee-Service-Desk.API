using EmployeeServiceDesk.Application.DTOs.Departments;

namespace EmployeeServiceDesk.Application.ServiceInterface
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();

        Task<DepartmentDto?> GetByIdAsync(int id);

        Task<DepartmentDto> CreateAsync(
            CreateDepartmentDto dto);

        Task<DepartmentDto?> UpdateAsync(
            int id,
            CreateDepartmentDto dto);

        Task<bool> DeactivateAsync(int id);
    }
}