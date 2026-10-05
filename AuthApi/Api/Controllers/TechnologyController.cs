using AuthApi.Application.Portfolio.Technologies.DTOs;
using AuthApi.Application.Portfolio.Technologies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthApi.Api.Controllers;

[ApiController]
[Route("api/technologies")]
public class TechnologyController : ControllerBase
{
    private readonly ITechnologyService _technologyService;
    private readonly ILogger<TechnologyController> _logger;
    public TechnologyController(
        ITechnologyService technologyService,
        ILogger<TechnologyController> logger)
    {
        _technologyService = technologyService;
        _logger = logger;
    }

    // GET /api/technologies
    [HttpGet]
    [EnableRateLimiting("api-moderate")]
    public async Task<ActionResult<List<TechnologiesListResponse>>> GetAllTechnologies(
        CancellationToken cancellationToken)
    {
        var result = await _technologyService
            .GetAllTechnologiesAsync(cancellationToken);

        var response = result.Data;

        return Ok(response);
    }

    // GET /api/technologies/5
    [HttpGet("{id:long:min(1)}")]
    [EnableRateLimiting("api-moderate")]
    public async Task<ActionResult<TechnologyResponse>> GetTechnologyById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _technologyService
            .GetTechnologyByIdAsync(id, cancellationToken);

        if (result is null)
        {
            _logger.LogWarning("[TechnologyController:GetTechnologyById] - Technology {id} not found.",
                id);

            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Technology {id} not found.",
                detail: null);
        }

        return Ok(result);
    }
}
