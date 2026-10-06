using Elsa.Studio.Contracts;

namespace ElsaStudio.Features;

public sealed class ShamlStudioFeature : IFeature
{
    public ValueTask InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }
}