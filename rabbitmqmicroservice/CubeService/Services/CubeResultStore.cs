public sealed class CubeResultStore
{
    private readonly Lock _lock = new();
    private ComputationResult? _latest;

    public void SetLatest(int input, int output, string correlationId)
    {
        lock (_lock)
        {
            _latest = new ComputationResult(input, output, correlationId, DateTimeOffset.UtcNow);
        }
    }

    public ComputationResult? GetLatest()
    {
        lock (_lock)
        {
            return _latest;
        }
    }
}
