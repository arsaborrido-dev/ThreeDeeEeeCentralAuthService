
using Microsoft.EntityFrameworkCore;
using AuthApi.Domain.Entitities.Portfolio;
using AuthApi.Application.Portfolio.Projects;

namespace AuthApi.Infrastracture.Persistence.Repositories
{
    public sealed class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllProjectsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.Skills)
                .Include(p => p.Technologies)
                .Include(p => p.Screenshots)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Project>> GetFeaturedProjectsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .AsNoTracking()
                .AsSplitQuery()
                .Where(p => p.IsFeatured)
                .Include(p => p.Skills)
                .Include(p => p.Technologies)
                .Include(p => p.Screenshots)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<Project?> GetProjectByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.Skills)
                .Include(p => p.Technologies)
                .Include(p => p.Screenshots)
                .FirstOrDefaultAsync(
                    p => p.ProjectId == id,
                    cancellationToken);
        }
    }    
}
