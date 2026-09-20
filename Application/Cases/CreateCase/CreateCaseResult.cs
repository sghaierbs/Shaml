namespace Application.Cases.CreateCase;

public sealed record CreateCaseResult(
    Guid Id,
    string CaseNumber
);