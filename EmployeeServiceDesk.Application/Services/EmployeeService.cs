using AutoMapper;
using EmployeeServiceDesk.Application.DTOs.Employees;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;

namespace EmployeeServiceDesk.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;

        public EmployeeService(
            IEmployeeRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var employees = await _repository.GetAllAsync(
                cancellationToken);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var employee = await _repository.GetByIdAsync(
                id,
                cancellationToken);

            return employee is null
                ? null
                : _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateAsync(
            CreateEmployeeDto dto,
            CancellationToken cancellationToken = default)
        {
            var employee = _mapper.Map<Employee>(dto);

            await _repository.AddAsync(
                employee,
                cancellationToken);

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto?> UpdateAsync(
            int id,
            UpdateEmployeeDto dto,
            CancellationToken cancellationToken = default)
        {
            var employee = await _repository.GetByIdAsync(
                id,
                cancellationToken);

            if (employee is null)
            {
                return null;
            }

            _mapper.Map(dto, employee);

            await _repository.UpdateAsync(
                employee,
                cancellationToken);

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> DeactivateAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var employee = await _repository.GetByIdAsync(
                id,
                cancellationToken);

            if (employee is null)
            {
                return false;
            }

            employee.IsActive = false;

            await _repository.UpdateAsync(
                employee,
                cancellationToken);

            return true;
        }
    }
}