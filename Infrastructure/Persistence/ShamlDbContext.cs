using Application.Common.Events;
using Application.Common.Interfaces;
using Domain.Cases;
using Domain.Centers;
using Domain.Common;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class ShamlDbContext : DbContext, IUnitOfWork
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    
    public ShamlDbContext(DbContextOptions<ShamlDbContext> options,IDomainEventDispatcher domainEventDispatcher): base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Case> Cases => Set<Case>();
    public DbSet<Center> Centers => Set<Center>();
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ShamlDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        var aggregateRoots = ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }

        await _domainEventDispatcher.DispatchAsync(
            domainEvents,
            cancellationToken);

        return result;
        
    }
}