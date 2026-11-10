using AuthApi.Application.Portfolio.Projects;
using AuthApi.Application.Portfolio.Projects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthApi.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController> _logger;
    public ProjectController(
        IProjectService projectService,
        ILogger<ProjectController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    // GET /api/projects
    [HttpGet]
    [EnableRateLimiting("api-moderate")]
    public async Task<ActionResult<List<ProjectResponse>>> GetAllProjects(
        CancellationToken cancellationToken = default)
    {
        var result = await _projectService
            .GetAllProjectsAsync(cancellationToken);

        //if (!result.Succeeded)
        //{
        //    _logger.LogWarning("[ProjectController:GetAllProjects] - All projects pulling failed.");

        //    return Problem(
        //        statusCode: StatusCodes.Status400BadRequest,
        //        title: "All projects pulling failed.",
        //        detail: _environment.IsDevelopment()
        //            ? result.ErrorMessage
        //            : null);
        //}

        var response = result.Data;

        return Ok(response);
    }

    // GET /api/projects/featured
    [HttpGet("featured")]
    [EnableRateLimiting("api-moderate")]
    public async Task<ActionResult<List<ProjectResponse>>> GetFeaturedProjects(
        CancellationToken cancellationToken)
    {
        var result = await _projectService
            .GetFeaturedProjectsAsync(cancellationToken);

        //if (!result.Succeeded)
        //{
        //    _logger.LogWarning("[ProjectController:GetAllProjects] - Featured projects pulling failed.");

        //    return Problem(
        //        statusCode: StatusCodes.Status400BadRequest,
        //        title: "Featured projects pulling failed.",
        //        detail: _environment.IsDevelopment()
        //            ? result.ErrorMessage
        //            : null);
        //}

        var response = result.Data;

        return Ok(response);
    }

    // GET /api/projects/5
    [HttpGet("{id:long:min(1)}")]
    [EnableRateLimiting("api-moderate")]
    [Authorize]
    public async Task<ActionResult<ProjectResponse>> GetProjectById(
        long id,
        CancellationToken cancellationToken)
    {

        var result = await _projectService
            .GetProjectByIdAsync(id, cancellationToken);

        if (result is null)
        {
            //return NotFound();
            _logger.LogWarning("[ProjectController:GetProjectById] - Project {id} not found.",
                id);

            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Project {id} not found.",
                detail: null);
        }

        return Ok(result);
    }

    //[HttpPut("update")]
    //[EnableRateLimiting("api-moderate")]
    //[Authorize]
    //public async Task<ActionResult> UpdateProjectAsync(ProjectResponse projDto, CancellationToken cancellationToken = default)
    //{
    //    await _projectService.UpdateProjectasync(projDto, cancellationToken);

    //    return Ok();
    //}


    [HttpPut("update/{id:long:min(1)}")]
    [EnableRateLimiting("api-moderate")]
    //[Authorize(Policy = "AdminOnly")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProjectAsync(
        long id,
        [FromBody] ProjectResponse request,
        CancellationToken cancellationToken = default)
    {
        if (id != request.Id)
        {
            return BadRequest("The route ID does not match the project ID.");
        }

        var updated = await _projectService.UpdateProjectAsync(
            request,
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }


    //[HttpPost("add")]
    //[EnableRateLimiting("api-moderate")]
    //[Authorize]
    //public async Task<ActionResult> AddProjectasync(ProjectResponse projDto, CancellationToken cancellationToken = default)
    //{
    //    await _projectService.AddProjectAsync(projDto, cancellationToken);

    //    return Ok();
    //}

    [HttpPost("add")]
    [EnableRateLimiting("api-moderate")]
    [Authorize] //(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddProjectAsync(
        [FromBody] ProjectResponse request,
        CancellationToken cancellationToken = default)
    {
        var createdProject = await _projectService.AddProjectAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetProjectById),
            new { id = createdProject.Id },
            createdProject);
    }


    //[HttpPut("delete/{id:long:min(1)}")]
    //[EnableRateLimiting("api-moderate")]
    //[Authorize]
    //public async Task<ActionResult> DeleteProjectAsync(long projId, CancellationToken cancellationToken = default)
    //{
    //    await _projectService.DeleteProjectAsync(projId, cancellationToken);

    //    return Ok();
    //}

    [HttpPut("delete/{id:long:min(1)}")]
    [EnableRateLimiting("api-moderate")]
    //[Authorize(Policy = "AdminOnly")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProjectAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _projectService.DeleteProjectAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

}
