using AuthApi.Application.Portfolio.Projects;
using AuthApi.Application.Portfolio.Projects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthApi.Api.Controllers
{
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
            CancellationToken cancellationToken)
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
        [HttpGet("{id:int:min(1)}")]
        [EnableRateLimiting("api-moderate")]
        public async Task<ActionResult<ProjectResponse>> GetProjectById(
            int id,
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
    }
}
