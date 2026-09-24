using AuthApi.Application.Auth.DTOs;
using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Projects.DTOs;

namespace AuthApi.Application.Portfolio.Projects
{
    public interface IProjectService
    {
        //Task<ProjectResultDto> GetAllProjectsAsync(
        //    CancellationToken cancellationToken = default);

        //Task<ProjectResultDto?> GetProjectByIdAsync(
        //    int id,
        //    CancellationToken cancellationToken = default);

        //Task<ProjectResultDto> GetFeaturedProjectsAsync(
        //    CancellationToken cancellationToken = default);

        Task<ResponseResult<List<ProjectResponse>>> GetAllProjectsAsync(
            CancellationToken cancellationToken = default);

        Task<ResponseResult<List<ProjectResponse>>> GetFeaturedProjectsAsync(
            CancellationToken cancellationToken = default); 
        Task<ResponseResult<ProjectResponse>?> GetProjectByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

    }
}
