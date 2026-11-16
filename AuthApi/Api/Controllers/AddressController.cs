using AuthApi.Application.Portfolio.Address;
using AuthApi.Application.Portfolio.Address.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthApi.Api.Controllers;

[Route("api/addresses")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly ILogger<AddressController> _logger;
    private readonly IAddressService _addSrvc;
    public AddressController(IAddressService addSrvc, ILogger<AddressController> logger)
    {
        _addSrvc = addSrvc;
        _logger = logger;
    }

    // GET /api/addresses/5
    [HttpGet("{id:long:min(1)}", Name = "GetAddressById")]
    [EnableRateLimiting("api-moderate")]
    [Authorize]
    public async Task<ActionResult<AddressResponseDto>> GetAddressByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var result = await _addSrvc.GetAddressByIdAsync(id, cancellationToken);

        if (result is null)
        {
            //log as error then return NotFound();
            _logger.LogWarning("[AddressController:GetAddressByIdAsync] - Address {id} not found.", id);

            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Address {id} not found.",
                detail: null);
        }

        return Ok(result);
    }

    // GET /api/useraddress/5
    [HttpGet("user/{id}")]
    [EnableRateLimiting("api-moderate")]
    [Authorize]
    public async Task<ActionResult<List<AddressResponseDto>>> GetAddressesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var result = await _addSrvc.GetAddressesByUserIdAsync(userId, cancellationToken);

        var data = result.Data;

        return Ok(data);
    }

    // GET /api/addresses
    [HttpGet]
    [EnableRateLimiting("api-strict")]
    [Authorize]
    public async Task<ActionResult<List<AddressResponseDto>>> GetAllAddressesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _addSrvc.GetAllAddressesAsync(cancellationToken);

        var data = result.Data;

        return Ok(data);
    }

    //DELETE
    [HttpPut("delete/{id:long:min(1)}")]
    [EnableRateLimiting("api-moderate")]
    [Authorize]
    public async Task<IActionResult> DeleteAddressAsync(long id, CancellationToken cancellationToken = default)
    {
        var deleted = await _addSrvc.DeleteAddressAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    //UPDATE
    [HttpPut("update/{id:long:min(1)}")]
    [EnableRateLimiting("api-moderate")]
    [Authorize]
    public async Task<IActionResult> UpdateAddressAsync(long id, [FromBody] AddressResponseDto request, CancellationToken cancellationToken = default)
    {
        if (id != request.Id)
        {
            return BadRequest("The route ID does not match the address Id");
        }

        var updated = await _addSrvc.UpdateAddressAsync(request, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("add")]
    [EnableRateLimiting("api-moderate")]
    [Authorize]
    public async Task<IActionResult> AddAddressAsync([FromBody] AddressResponseDto request, CancellationToken cancellationToken = default)
    {
        var createdAddress = await _addSrvc.AddAddressAsync(request, cancellationToken);

        //return CreatedAtAction(nameof(GetAddressByIdAsync), new { id = createdAddress.Id }, createdAddress);
        return CreatedAtRoute("GetAddressById", new { id = createdAddress.Id }, createdAddress);
    }
}
