using Application.Common.Interfaces;
using Domain.Assignments;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class AssignmentRepository : IAssignmentRepository
{
    private readonly ShamlDbContext _dbContext;

    public AssignmentRepository(
        ShamlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Assignment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Assignments
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Assignment assignment,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Assignments.AddAsync(
            assignment,
            cancellationToken);
    }
}