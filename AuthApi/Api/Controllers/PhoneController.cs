using AuthApi.Application.Portfolio.Address.DTOs;
using AuthApi.Application.Portfolio.Phone;
using AuthApi.Application.Portfolio.Phone.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthApi.Api.Controllers;


[Route("api/phones")]
[ApiController]
public class PhoneController : ControllerBase
{

    private readonly IPhoneService _phoneSrvc;

    public PhoneController(IPhoneService phoneSrvc) => _phoneSrvc = phoneSrvc;

    [HttpPut("delete/{id:long:min(1)}")]
    [Authorize]
    [EnableRateLimiting("api-strict")]
    public async Task<IActionResult> DeletePhoneAsync(long id, CancellationToken cancellationToken = default)
    {
        var deleted = await _phoneSrvc.DeletePhoneAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("update/{id:long:min(1)}")]
    [EnableRateLimiting("api-strict")]
    [Authorize]
    public async Task<IActionResult> UpdatePhoneAsync(long id, PhoneResponseRequestDto request, CancellationToken cancellationToken = default)
    {
        if (id != request.Id)
        {
            return BadRequest("The route ID does not match the phone Id");
        }

        var updated = await _phoneSrvc.UpdatePhoneAsync(request, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();

    }

    [HttpPost("add")]
    [Authorize]
    [EnableRateLimiting("api-strict")]
    public async Task<IActionResult> AddPhoneAsync(PhoneResponseRequestDto request, CancellationToken cancellationToken = default)
    {
        var createdPhone = await _phoneSrvc.AddPhoneAsync(request, cancellationToken);

        return CreatedAtRoute("GetPhonById", new { id = createdPhone.Id }, createdPhone);
    }

    [HttpGet]
    [Authorize]
    [EnableRateLimiting("api-strict")]
    public async Task<ActionResult<List<PhoneResponseRequestDto>>> GetAllPhonesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _phoneSrvc.GetAllPhonesAsync(cancellationToken);

        var data = response.Data;
        return Ok(data);
    }


    //[HttpGet]
    //[EnableRateLimiting("api-strict")]
    //[Authorize]
    //public async Task<ActionResult<List<AddressResponseDto>>> GetAllAddressesAsync(CancellationToken cancellationToken = default)
    //{
    //    var result = await _addSrvc.GetAllAddressesAsync(cancellationToken);

    //    var data = result.Data;

    //    return Ok(data);
    //}



    [HttpGet("phone/{id}")]
    [EnableRateLimiting("api-moderate")]
    [Authorize]
    public async Task<IActionResult> GetPhoneByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await _phoneSrvc.GetPhoneByIdAsync(id, cancellationToken);
        return Ok(response);
    }
}
