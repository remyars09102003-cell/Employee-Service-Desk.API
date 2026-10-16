using EmployeeServiceDesk.Application.DTOs.SLA;
using EmployeeServiceDesk.Application.Interfaces;
using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Enums;
using EmployeeServiceDesk.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeServiceDesk.Application.Services;

public class SLAService : ISLAService
{
    private readonly ISLARepository _repository;
    private readonly ILogger<SLAService> _logger;

    public SLAService(ISLARepository repository,ILogger<SLAService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<SlaDto>> GetAllAsync()
    {
        try
        {
            var slas = await _repository.GetAllAsync();

            return slas.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting SLA list");
            throw;
        }
    }

    public async Task<SlaDto?> GetByIdAsync(int id)
    {
        try
        {
            var sla = await _repository.GetByIdAsync(id);

            if (sla == null)
                return null;

            return MapToDto(sla);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while getting SLA {Id}",id);

            throw;
        }
    }

    public async Task<SlaDto> CreateAsync(CreateSlaDto dto)
    {
        try
        {
            if (dto.ResponseTimeMinutes <= 0)
                throw new ArgumentException("Response time must be greater than zero.");

            if (dto.ResolutionTimeMinutes <= 0)
                throw new ArgumentException("Resolution time must be greater than zero.");

            var existing = await _repository.GetByPriorityAsync(dto.Priority);

            if (existing != null)
            {
                throw new InvalidOperationException($"An active SLA already exists for {dto.Priority} priority.");
            }

            var sla = new SLA
            {
                Name = dto.Name,
                Priority = dto.Priority,
                ResponseTimeMinutes = dto.ResponseTimeMinutes,
                ResolutionTimeMinutes = dto.ResolutionTimeMinutes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(sla);

            return MapToDto(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating SLA");
            throw;
        }
    }

    public async Task<SlaDto?> UpdateAsync(int id,UpdateSlaDto dto)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                return null;

            existing.Name = dto.Name;
            existing.Priority = dto.Priority;
            existing.ResponseTimeMinutes = dto.ResponseTimeMinutes;

            existing.ResolutionTimeMinutes = dto.ResolutionTimeMinutes;

            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing);

            return MapToDto(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while updating SLA {Id}",
                id);

            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            return await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while deleting SLA {Id}",
                id);

            throw;
        }
    }

    public async Task<(DateTime ResponseDeadline,DateTime ResolutionDeadline)>CalculateDeadlineAsync(Priority priority)
    {
        try
        {
            var sla = await _repository.GetByPriorityAsync(priority);

            if (sla == null)
            {
                throw new InvalidOperationException(
                    $"No active SLA configured for {priority} priority.");
            }

            var now = DateTime.UtcNow;

            var responseDeadline = now.AddMinutes(sla.ResponseTimeMinutes);

            var resolutionDeadline = now.AddMinutes(sla.ResolutionTimeMinutes);

            return (
                responseDeadline,
                resolutionDeadline
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while calculating SLA deadline");

            throw;
        }
    }

    private static SlaDto MapToDto(SLA sla)
    {
        return new SlaDto
        {
            Id = sla.Id,
            Name = sla.Name,
            Priority = sla.Priority,
            ResponseTimeMinutes = sla.ResponseTimeMinutes,
            ResolutionTimeMinutes = sla.ResolutionTimeMinutes,
            IsActive = sla.IsActive,
            CreatedAt = sla.CreatedAt
        };
    }
}