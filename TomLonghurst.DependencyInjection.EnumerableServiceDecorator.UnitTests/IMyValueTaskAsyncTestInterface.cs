namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.UnitTests;

public interface IMyValueTaskAsyncTestInterface
{
    public ValueTask BlahAsync(Func<string, ValueTask> action);
}

public class MyValueTaskAsyncTestClass : IMyValueTaskAsyncTestInterface
{
    public async ValueTask BlahAsync(Func<string, ValueTask> action)
    {
        await action(GetType().Name);
    }
}
public class MyValueTaskAsyncTestClass2 : IMyValueTaskAsyncTestInterface
{
    public async ValueTask BlahAsync(Func<string, ValueTask> action)
    {
        await action(GetType().Name);
    }
}
public class MyValueTaskAsyncTestClass3 : IMyValueTaskAsyncTestInterface
{
    public async ValueTask BlahAsync(Func<string, ValueTask> action)
    {
        await action(GetType().Name);
    }
}