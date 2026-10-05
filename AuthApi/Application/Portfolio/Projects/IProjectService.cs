
using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Projects.DTOs;

namespace AuthApi.Application.Portfolio.Projects;
public interface IProjectService
{
    Task<ResponseResult<List<ProjectResponse>>> GetAllProjectsAsync(
        CancellationToken cancellationToken = default);
    Task<ResponseResult<List<ProjectResponse>>> GetFeaturedProjectsAsync(
        CancellationToken cancellationToken = default); 
    Task<ResponseResult<ProjectResponse>?> GetProjectByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
}
