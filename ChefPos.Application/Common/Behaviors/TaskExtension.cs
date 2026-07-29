namespace ChefPos.Application.Common.Behaviors;

public static class TaskExtensions
{
    public static async Task<T> OrThrowNotFoundAsync<T>(this Task<T?> task, string message) where T : class
    {
        var result = await task;
        if (result is null)
        {
            throw new KeyNotFoundException(message);
        }
        return result;
    }
}