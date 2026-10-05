using AuthApi.Application.Portfolio.Technologies;
using AuthApi.Application.Portfolio.Technologies.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Infrastracture.Persistence.Repositories.Portfolio;
public sealed class TechnologyRepository : ITechnologyRepository
{
    private readonly ApplicationDbContext _context;
    public TechnologyRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<TechnologiesListResponse>> GetAllTechnologiesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Technologies
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TechnologiesListResponse
            {
                Id = t.TechnologyId,
                Name = t.Name
            })
            .ToListAsync(cancellationToken);
    }
    public async Task<TechnologyResponse?> GetTechnologyByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Technologies
            .AsNoTracking()
            .Where(t => t.TechnologyId == id)
            .Select(t => new TechnologyResponse
            {
                Id = t.TechnologyId,
                Name = t.Name,
                Description = t.Description,

                Projects = t.Projects
                    .OrderByDescending(p => p.CreatedAtUtc)
                    .Select(p => new TechnologyProjectSummary
                    {
                        Id = p.ProjectId,
                        ShortDescription = p.ShortDescription,
                        Title = p.Title,
                        ThumbnailImageUrl = p.ThumbnailImageUrl
                    }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);
    }
}
