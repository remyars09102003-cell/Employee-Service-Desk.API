using EmployeeServiceDesk.Application.DTOs.Asset;

namespace EmployeeServiceDesk.Application.ServiceInterface;

public interface IAssetService
{
    Task<List<AssetDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<AssetDto?> GetByIdAsync(
        int assetId,
        CancellationToken cancellationToken = default);

    Task<AssetDto> CreateAsync(
        CreateAssetDto dto,
        CancellationToken cancellationToken = default);

    Task<AssetDto?> UpdateAsync(
        int assetId,
        UpdateAssetDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int assetId,
        CancellationToken cancellationToken = default);

    Task<bool> AssignAsync(
        int assetId,
        AssignAssetDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> ReturnAsync(
        int assetId,
        CancellationToken cancellationToken = default);
}