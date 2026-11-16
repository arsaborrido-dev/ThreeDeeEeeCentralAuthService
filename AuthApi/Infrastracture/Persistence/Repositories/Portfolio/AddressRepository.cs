using AuthApi.Application.Portfolio.Address;
using AuthApi.Application.Portfolio.Address.DTOs;
using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Infrastracture.Persistence.Repositories.Portfolio;

public sealed class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;
    public AddressRepository(ApplicationDbContext context) => _context = context;

    public async Task<AddressResponseDto?> GetAddressByIdAsync(long addressId, CancellationToken cancellationToken = default)
    {
        //use Projection
        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.AddressId == addressId)
            .Select(
                n => new AddressResponseDto
                {
                    Id = n.AddressId,
                    AddressLine1 = n.AddressLine1,
                    ZipCode = n.ZipCode,
                    CreatedAtUtc = n.CreatedAtUtc,
                    UpdatedAtUtc = n.UpdatedAtUtc,
                    UserId = n.UserId
                }
            ).FirstOrDefaultAsync(cancellationToken);

    }

    public async Task<List<AddressResponseDto>> GetAddressesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(
                n => new AddressResponseDto
                {
                    Id = n.AddressId,
                    AddressLine1 = n.AddressLine1,
                    ZipCode = n.ZipCode,
                    CreatedAtUtc = n.CreatedAtUtc,
                    UpdatedAtUtc = n.UpdatedAtUtc,
                    UserId = n.UserId
                }
            ).ToListAsync(cancellationToken);
    }

    public async Task<List<AddressResponseDto>> GetAllAddressesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Addresses
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(
                n => new AddressResponseDto
                {
                    Id = n.AddressId,
                    AddressLine1 = n.AddressLine1,
                    ZipCode = n.ZipCode,
                    CreatedAtUtc = n.CreatedAtUtc,
                    UpdatedAtUtc = n.UpdatedAtUtc,
                    UserId = n.UserId
                }
            ).ToListAsync();
    }

    //CREATE
    public async Task<AddressResponseDto> AddAddressAsync(AddressResponseDto address, CancellationToken cancellationToken = default)
    {
        var entAddress = new Address
        {
            AddressLine1 = address.AddressLine1,
            ZipCode = address.ZipCode,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
            UserId = address.UserId
        };

        _context.Addresses.Add(entAddress);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entAddress);
    }


    //UPDATE
    public async Task<bool> UpdateAddressAsync(AddressResponseDto address, CancellationToken cancellationToken = default)
    {
        var entAddress = await _context.Addresses.FindAsync(address.Id);

        if (entAddress is null)
        {
            return false;
        }

        entAddress.AddressLine1 = address.AddressLine1;
        entAddress.ZipCode = address.ZipCode;
        entAddress.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SoftDeleteAddressAsync(long id, CancellationToken cancellationToken = default)
    {
        var entAddress = await _context.Addresses.FindAsync(id);

        if (entAddress is null)
        {
            return false;
        }

        entAddress.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static AddressResponseDto MapToDto(Address address)
    {
        return new AddressResponseDto
        {
            Id = address.AddressId,
            AddressLine1 = address.AddressLine1,
            ZipCode = address.ZipCode,
            CreatedAtUtc = address.CreatedAtUtc,
            UpdatedAtUtc = address.UpdatedAtUtc
        };
    }
}
