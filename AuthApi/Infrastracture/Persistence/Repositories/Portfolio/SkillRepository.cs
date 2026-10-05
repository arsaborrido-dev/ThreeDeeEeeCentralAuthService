
using Microsoft.EntityFrameworkCore;
using AuthApi.Application.Portfolio.Skills.DTOs;
using AuthApi.Application.Portfolio.Skills;

namespace AuthApi.Infrastracture.Persistence.Repositories.Portfolio;
public sealed class SkillRepository : ISkillRepository
{
    private readonly ApplicationDbContext _context;
    public SkillRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<SkillsListResponse>> GetAllSkillsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SkillsListResponse
            {
                Id = s.SkillId,
                Name = s.Name,
                Description = s.Description,
                SkillCategory = s.SkillCategory.Name,
            })
            .ToListAsync(cancellationToken);
    }
    public async Task<SkillResponse?> GetSkillByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .AsNoTracking()
            .Where(s => s.SkillId == id)
            .Select(s => new SkillResponse
            {
                Id = s.SkillId,
                Name = s.Name,
                Description = s.Description,
                SkillCategory = s.SkillCategory.Name,

                Projects = s.Projects
                    .OrderByDescending(p => p.CreatedAtUtc)
                    .Select(p => new SkillProjectSummary
                    {
                        Id = p.ProjectId,
                        Title = p.Title,
                        ShortDescription = p.ShortDescription,
                        ThumbnailImageUrl = p.ThumbnailImageUrl
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
