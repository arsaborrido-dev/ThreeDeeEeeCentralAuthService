using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Contacts.DTOs;

namespace AuthApi.Application.Portfolio.Contacts
{
    public interface IContactService
    {
        Task<ResponseResult> SaveContactMessageAsync(
            ContactMessageRequest request,
            CancellationToken cancellationToken = default);
    }
}
