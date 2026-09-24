using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Projects.DTOs;
using AuthApi.Domain.Entitities.Portfolio;

namespace AuthApi.Application.Portfolio.Projects
{
    public sealed class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;       
        public ProjectService(
            IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<ResponseResult<List<ProjectResponse>>> GetAllProjectsAsync(
            CancellationToken cancellationToken = default)
        {
            var projects = await _projectRepository
                .GetAllProjectsAsync(cancellationToken);

            var response = projects
                .Select(MapToDto)
                .ToList();

            return ResponseResult<List<ProjectResponse>>.Success(response);

        }

        public async Task<ResponseResult<List<ProjectResponse>>> GetFeaturedProjectsAsync(
            CancellationToken cancellationToken = default)
        {
            var projects = await _projectRepository
                .GetFeaturedProjectsAsync(cancellationToken);

            var response = projects
                .Select(MapToDto)
                .ToList();

            return ResponseResult<List<ProjectResponse>>.Success(response);
        }

        public async Task<ResponseResult<ProjectResponse>?> GetProjectByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository
                .GetProjectByIdAsync(id, cancellationToken);

            var response = project is null
                ? null
                : MapToDto(project);

            return ResponseResult<ProjectResponse>.Success(response);
        }

        private static ProjectResponse MapToDto(Project project)
        {
            return new ProjectResponse
            {
                Id = project.ProjectId,
                Title = project.Title,
                ShortDescription = project.ShortDescription,
                LongDescription = project.LongDescription,

                ThumbnailImageUrl = project.ThumbnailImageUrl,

                GithubUrl = project.GitHubUrl,
                LiveDemoUrl = project.LiveDemoUrl,

                CreatedAtUtc = project.CreatedAtUtc,
                IsFeatured = project.IsFeatured,

                ScreenshotUrls = project.Screenshots
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => s.ImageUrl)
                    .ToList(),

                Skills = project.Skills
                    .OrderBy(s => s.Name)
                    .Select(s => s.Name)
                    .ToList(),

                Technologies = project.Technologies
                    .OrderBy(t => t.Name)
                    .Select(t => t.Name)
                    .ToList()
            };
        }
    }    
}
