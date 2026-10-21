using AutoMapper;
using EmployeeServiceDesk.Application.DTOs.Departments;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;

namespace EmployeeServiceDesk.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;

        public DepartmentService(
            IDepartmentRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<DepartmentDto>>(
                departments);
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department is null)
            {
                return null;
            }

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto> CreateAsync(
            CreateDepartmentDto dto)
        {
            var department = _mapper.Map<Department>(dto);

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto?> UpdateAsync(
            int id,
            CreateDepartmentDto dto)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department is null)
            {
                return null;
            }

            _mapper.Map(dto, department);

            _repository.Update(department);

            await _repository.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department is null)
            {
                return false;
            }

            department.IsActive = false;

            _repository.Update(department);

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}