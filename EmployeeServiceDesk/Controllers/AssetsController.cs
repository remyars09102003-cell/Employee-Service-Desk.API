using EmployeeServiceDesk.Application.DTOs.Asset;
using EmployeeServiceDesk.Application.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    // GET: api/assets
    [HttpGet]
    public async Task<ActionResult<List<AssetDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var assets = await _assetService.GetAllAsync(cancellationToken);

        return Ok(assets);
    }

    // GET: api/assets/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssetDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var asset = await _assetService.GetByIdAsync(
            id,
            cancellationToken);

        if (asset == null)
        {
            return NotFound(new
            {
                message = "Asset not found."
            });
        }

        return Ok(asset);
    }

    // POST: api/assets
    [HttpPost]
    public async Task<ActionResult<AssetDto>> Create(
        [FromBody] CreateAssetDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var asset = await _assetService.CreateAsync(
                dto,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = asset.AssetId },
                asset);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: api/assets/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<AssetDto>> Update(
        int id,
        [FromBody] UpdateAssetDto dto,
        CancellationToken cancellationToken)
    {
        var asset = await _assetService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        if (asset == null)
        {
            return NotFound(new
            {
                message = "Asset not found."
            });
        }

        return Ok(asset);
    }

    // DELETE: api/assets/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _assetService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Asset not found."
            });
        }

        return NoContent();
    }

    // POST: api/assets/5/assign
    [HttpPost("{id:int}/assign")]
    public async Task<IActionResult> Assign(
        int id,
        [FromBody] AssignAssetDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var assigned = await _assetService.AssignAsync(
                id,
                dto,
                cancellationToken);

            if (!assigned)
            {
                return NotFound(new
                {
                    message = "Asset not found."
                });
            }

            return Ok(new
            {
                message = "Asset assigned successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // POST: api/assets/5/return
    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> Return(
        int id,
        CancellationToken cancellationToken)
    {
        var returned = await _assetService.ReturnAsync(
            id,
            cancellationToken);

        if (!returned)
        {
            return NotFound(new
            {
                message = "Asset not found."
            });
        }

        return Ok(new
        {
            message = "Asset returned successfully."
        });
    }
}