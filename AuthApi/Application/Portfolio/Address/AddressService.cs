using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Address.DTOs;
using AuthApi.Application.Portfolio.Projects.DTOs;

namespace AuthApi.Application.Portfolio.Address;

public sealed class AddressService : IAddressService
{
    private readonly IAddressRepository _addRepo;
    public AddressService(IAddressRepository addRepo) => _addRepo = addRepo;

    public async Task<ResponseResult<AddressResponseDto>?> GetAddressByIdAsync(long addressId, CancellationToken cancellationToken = default)
    {
        var result = await _addRepo.GetAddressByIdAsync(addressId, cancellationToken);

        return ResponseResult<AddressResponseDto>.Success(result);
    }

    public async Task<ResponseResult<List<AddressResponseDto>>> GetAddressesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var result = await _addRepo.GetAddressesByUserIdAsync(userId, cancellationToken);

        return ResponseResult<List<AddressResponseDto>>.Success(result);
    }

    public async Task<ResponseResult<List<AddressResponseDto>>> GetAllAddressesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _addRepo.GetAllAddressesAsync();

        return ResponseResult<List<AddressResponseDto>>.Success(result);
    }

    //UPDATE
    public async Task<bool> UpdateAddressAsync(AddressResponseDto address, CancellationToken cancellationToken = default)
    {
        return await _addRepo.UpdateAddressAsync(address, cancellationToken);
    }

    //ADD
    public async Task<AddressResponseDto> AddAddressAsync(AddressResponseDto address, CancellationToken cancellationToken = default)
    {
        return await _addRepo.AddAddressAsync(address, cancellationToken);
    }

    //DELETE
    public async Task<bool> DeleteAddressAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _addRepo.SoftDeleteAddressAsync(id, cancellationToken);
    }
}
