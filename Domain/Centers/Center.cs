using Domain.Common;
using Domain.Centers.Events;

namespace Domain.Centers;

public class Center : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Region { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }

    // Maximum number of active cases the center can handle concurrently.
    // Null means capacity has not yet been configured.
    public int? ActiveCaseCapacity { get; private set; }

    public CenterStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Center()
    {
    }

    private Center(
        Guid id,
        string code,
        string name,
        string region,
        string city,
        string? address,
        string? phone,
        string? email)
    {
        Id = id;
        Code = code;
        Name = name;
        Region = region;
        City = city;
        Address = address;
        Phone = phone;
        Email = email;

        Status = CenterStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Center Create(
        string code,
        string name,
        string region,
        string city,
        string? address = null,
        string? phone = null,
        string? email = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Center code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Center name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region is required.", nameof(region));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));

        var center = new Center(
            Guid.NewGuid(),
            code.Trim(),
            name.Trim(),
            region.Trim(),
            city.Trim(),
            address?.Trim(),
            phone?.Trim(),
            email?.Trim());

        center.RaiseDomainEvent(
            new CenterCreatedEvent(center.Id, center.Code));

        return center;
    }

    public void ConfigureActiveCaseCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");

        ActiveCaseCapacity = capacity;
    }

    public bool HasAvailableCapacity(int activeCaseCount)
    {
        if (activeCaseCount < 0)
            throw new ArgumentOutOfRangeException(nameof(activeCaseCount));

        if (ActiveCaseCapacity is null)
            throw new InvalidOperationException("Center active case capacity has not been configured.");

        return Status == CenterStatus.Active && activeCaseCount < ActiveCaseCapacity.Value;
    }

    public void Activate()
    {
        Status = CenterStatus.Active;
    }

    public void Deactivate()
    {
        Status = CenterStatus.Inactive;
    }
}