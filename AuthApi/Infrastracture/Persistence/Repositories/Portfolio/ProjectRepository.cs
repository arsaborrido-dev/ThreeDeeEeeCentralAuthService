using AuthApi.Application.Portfolio.Projects;
using AuthApi.Application.Portfolio.Projects.DTOs;
using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Infrastracture.Persistence.Repositories.Portfolio;
public sealed class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;
    public ProjectRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<ProjectResponse>> GetAllProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .OrderBy(p => p.Title)
                .Select(p => new ProjectResponse
                {
                    Id = p.ProjectId,
                    Title = p.Title,
                    ShortDescription = p.ShortDescription,
                    LongDescription = p.LongDescription,
                    ThumbnailImageUrl = p.ThumbnailImageUrl,
                    CreatedAtUtc = p.CreatedAtUtc,
                    GithubUrl = p.GitHubUrl,
                    LiveDemoUrl = p.LiveDemoUrl,
                    IsFeatured = p.IsFeatured,

                    Skills = p.Skills
                        .Select(s => new ProjectSkillSummary
                        {
                            Id = s.SkillId,
                            Name = s.Name
                        })
                        .ToList(),

                    Technologies = p.Technologies
                        .Select(t => new ProjectTechnologySummary
                        {
                            Id = t.TechnologyId,
                            Name = t.Name
                        })
                        .ToList(),

                    Screenshots = p.Screenshots
                        .Select(s => new ProjectScreenshotSummary
                        {
                            Id = s.ProjectScreenshotId,
                            ImageUrl = s.ImageUrl
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

        //return await _context.Projects
        //    .AsNoTracking()
        //    .AsSplitQuery()
        //    .Include(p => p.Skills)
        //    .Include(p => p.Technologies)
        //    .Include(p => p.Screenshots)
        //    .OrderByDescending(p => p.CreatedAtUtc)
        //    .ToListAsync(cancellationToken);
    }
    public async Task<List<ProjectResponse>> GetFeaturedProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .OrderBy(p => p.Title)
                .Select(p => new ProjectResponse
                {
                    Id = p.ProjectId,
                    Title = p.Title,
                    ShortDescription = p.ShortDescription,
                    //LongDescription = p.LongDescription,
                    ThumbnailImageUrl = p.ThumbnailImageUrl
                })
                .ToListAsync(cancellationToken);

        //return await _context.Projects
        //    .AsNoTracking()
        //    .AsSplitQuery()
        //    .Where(p => p.IsFeatured)
        //    .Include(p => p.Skills)
        //    .Include(p => p.Technologies)
        //    .Include(p => p.Screenshots)
        //    .OrderByDescending(p => p.CreatedAtUtc)
        //    .ToListAsync(cancellationToken);
    }
    public async Task<ProjectResponse?> GetProjectByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        //use Projection instead of using Include() with AsSplitQuery
        return await _context.Projects
        .AsNoTracking()
        .Where(p => p.ProjectId == id)
        .Select(p => new ProjectResponse
        {
            Id = p.ProjectId,
            Title = p.Title,
            ShortDescription = p.ShortDescription,
            LongDescription = p.LongDescription,
            ProjectDemonstration = p.ProjectDemonstration,
            ThumbnailImageUrl = p.ThumbnailImageUrl,
            GithubUrl = p.GitHubUrl,
            LiveDemoUrl = p.LiveDemoUrl,
            IsFeatured = p.IsFeatured,
            CreatedAtUtc = p.CreatedAtUtc,

            Skills = p.Skills
                .Select(s => new ProjectSkillSummary
                {
                    Id = s.SkillId,
                    Name = s.Name,
                    SkillCategory = s.SkillCategory.Name
                })
                .ToList(),

            Technologies = p.Technologies
                .Select(t => new ProjectTechnologySummary
                {
                    Id = t.TechnologyId,
                    Name = t.Name,
                    TechnologyCategory = t.TechnologyCategory.Name
                })
                .ToList(),

            Screenshots = p.Screenshots
                .Select(s => new ProjectScreenshotSummary
                {
                    Id = s.ProjectScreenshotId,
                    ImageUrl = s.ImageUrl
                })
                .ToList()
        })
        .FirstOrDefaultAsync(cancellationToken);

        //return await _context.Projects
        //    .AsNoTracking()
        //    .AsSplitQuery()
        //    .Include(p => p.Skills)
        //    .Include(p => p.Technologies)
        //    .Include(p => p.Screenshots)
        //    .FirstOrDefaultAsync(
        //        p => p.ProjectId == id,
        //        cancellationToken);
    }

    public async Task<bool> UpdateProjectAsync(ProjectResponse projDto, CancellationToken cancellationToken = default)
    {
        //var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projDto.Id, cancellationToken);
        var project = await _context.Projects.FindAsync(projDto.Id);

        if (project is null)
        {
            return false;
        }

        project.IsFeatured = projDto.IsFeatured;
        project.ShortDescription = projDto.ShortDescription;
        project.Title = projDto.Title;
        project.GitHubUrl = projDto.GithubUrl;
        project.LiveDemoUrl = projDto.LiveDemoUrl;

        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }
    public async Task<ProjectResponse> AddProjectAsync(ProjectResponse projDto, CancellationToken cancellationToken = default)
    {
        var entProject = new Project
        {
            IsFeatured = projDto.IsFeatured,
            ShortDescription = projDto.ShortDescription,
            Title = projDto.Title,
            GitHubUrl = projDto.GithubUrl,
            LiveDemoUrl = projDto.LiveDemoUrl
        };

        _context.Projects.Add(entProject);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToResponse(entProject);
    }

    public async Task<bool> SoftDeleteProjectAsync(long id, CancellationToken cancellationToken = default)
    {
        //var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id, cancellationToken);

        var project = await _context.Projects.FindAsync(id);

        if (project is null)
        {
            return false;
        }

        project.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }

    private static ProjectResponse MapToResponse(Project proj)
    {
        return new ProjectResponse
        {
            Id = proj.ProjectId,
            Title = proj.Title,
            ShortDescription = proj.ShortDescription,
            LongDescription = proj.LongDescription,
            ProjectDemonstration = proj.ProjectDemonstration,
            GithubUrl = proj.GitHubUrl,
            LiveDemoUrl = proj.LiveDemoUrl,
            ThumbnailImageUrl = proj.ThumbnailImageUrl
        };
    }
}    
