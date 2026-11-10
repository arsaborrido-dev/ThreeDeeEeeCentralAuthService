using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Address.DTOs;
using AuthApi.Application.Portfolio.Phone.DTOs;

namespace AuthApi.Application.Portfolio.Phone;

public sealed class PhoneService : IPhoneService
{
    private readonly IPhoneRepository _phoneRepo;

    public PhoneService(IPhoneRepository phoneRepo) => _phoneRepo = phoneRepo;
    public async Task<PhoneResponseRequestDto> AddPhoneAsync(PhoneResponseRequestDto dto, CancellationToken cancellationToken = default)
    {
        return await _phoneRepo.AddPhoneAsync(dto, cancellationToken);
    }

    public async Task<bool> DeletePhoneAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _phoneRepo.SoftDeletePhoneAsync(id, cancellationToken);
    }
    public async Task<ResponseResult<List<PhoneResponseRequestDto>>> GetAllPhonesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _phoneRepo.GetAllPhonesAsync(cancellationToken);

        return ResponseResult<List<PhoneResponseRequestDto>>.Success(result);

    }

    public async Task<PhoneResponseRequestDto?> GetPhoneByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _phoneRepo.GetPhoneByIdAsync(id, cancellationToken);
    }

    public async Task<bool> UpdatePhoneAsync(PhoneResponseRequestDto dto, CancellationToken cancellationToken = default)
    {
        return await _phoneRepo.UpdatePhoneAsync(dto, cancellationToken);
    }
}
