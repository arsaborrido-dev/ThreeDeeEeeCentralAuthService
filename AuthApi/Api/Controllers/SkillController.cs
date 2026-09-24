using AuthApi.Application.Portfolio.Skills;
using AuthApi.Application.Portfolio.Skills.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthApi.Api.Controllers
{
    [ApiController]
    [Route("api/skills")]
    public sealed class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;
        private readonly ILogger<ProjectController> _logger;

        public SkillController(
            ISkillService skillService,
            ILogger<ProjectController> logger)
        {
            _skillService = skillService;
            _logger = logger;
        }

        // GET /api/skills
        [HttpGet]
        [EnableRateLimiting("api-moderate")]
        public async Task<ActionResult<List<SkillListResponse>>> GetAllSkills(
            CancellationToken cancellationToken)
        {
            var result = await _skillService
                .GetAllSkillsAsync(cancellationToken);

            var response = result.Data;

            return Ok(response);
        }

        // GET /api/skills/5
        [HttpGet("{id:int:min(1)}")]
        [EnableRateLimiting("api-moderate")]
        public async Task<ActionResult<SkillResponse>> GetSkillById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _skillService
                .GetSkillByIdAsync(id, cancellationToken);

            if (result is null)
            {
                //return NotFound();
                _logger.LogWarning("[SkillController:GetSkillById] - Skill {id} not found.",
                    id);

                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Skill {id} not found.",
                    detail: null);
            }

            return Ok(result);
        }
    }    
}
