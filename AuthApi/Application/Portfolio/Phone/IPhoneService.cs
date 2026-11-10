using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Phone.DTOs;

namespace AuthApi.Application.Portfolio.Phone;

public interface IPhoneService
{
    Task<PhoneResponseRequestDto> AddPhoneAsync(PhoneResponseRequestDto dto, CancellationToken cancellationToken = default);
    Task<ResponseResult<List<PhoneResponseRequestDto>>> GetAllPhonesAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdatePhoneAsync(PhoneResponseRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeletePhoneAsync(long id, CancellationToken cancellationToken = default);
    Task<PhoneResponseRequestDto?> GetPhoneByIdAsync(long id, CancellationToken cancellationToken = default);
}
