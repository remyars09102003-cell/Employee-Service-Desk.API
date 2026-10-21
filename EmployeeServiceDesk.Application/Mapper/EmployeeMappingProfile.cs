using AutoMapper;
using EmployeeServiceDesk.Application.DTOs.Employees;
using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Application.Mapper;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        // Entity to response DTO
        CreateMap<Employee, EmployeeDto>();

        // Create DTO to entity
        CreateMap<CreateEmployeeDto, Employee>();

        // Update DTO to entity
        CreateMap<UpdateEmployeeDto, Employee>();
    }
}