namespace MathGame.Services.Services;
public class Lock
{
    private readonly SemaphoreSlim _lock;

    private Lock()
    {
        _lock = new SemaphoreSlim(1);
    }

    public async Task Acquire()
    {

    }
}
