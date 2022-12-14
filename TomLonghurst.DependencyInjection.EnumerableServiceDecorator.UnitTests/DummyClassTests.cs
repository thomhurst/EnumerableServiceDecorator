using Microsoft.Extensions.DependencyInjection;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.UnitTests;

public class DummyClassTests
{
    [Test]
    public void DummyClass()
    {
        var services = new ServiceCollection()
            .AddTransient<IMyDummyTestInterface, MyTransientTestClass>()
            .AddScoped<IMyDummyTestInterface, MyScopedTestClass>()
            .AddSingleton<IMyDummyTestInterface, MySingletonTestClass>()
            .FlattenEnumerableToSingle<IMyDummyTestInterface>()
            .BuildServiceProvider();

        var scope = services.CreateScope().ServiceProvider;
        scope.GetRequiredService<IMyDummyTestInterface>().Blah();

        var transient = Get<MyTransientTestClass>(scope);
        var scoped = Get<MyScopedTestClass>(scope);
        var singleton = Get<MySingletonTestClass>(scope);

        Assert.That(transient.BlahCount, Is.EqualTo(0));
        Assert.That(scoped.BlahCount, Is.EqualTo(1));
        Assert.That(singleton.BlahCount, Is.EqualTo(1));

        scope.GetRequiredService<IMyDummyTestInterface>().Blah();

        Assert.That(transient.BlahCount, Is.EqualTo(0));
        Assert.That(scoped.BlahCount, Is.EqualTo(2));
        Assert.That(singleton.BlahCount, Is.EqualTo(2));

        var newScope = services.CreateScope().ServiceProvider;

        var newScopedTransient = Get<MyTransientTestClass>(newScope);
        var newScopedScoped = Get<MyScopedTestClass>(newScope);
        var newScopedSingleton = Get<MySingletonTestClass>(newScope);

        Assert.That(newScopedTransient.BlahCount, Is.EqualTo(0));
        Assert.That(newScopedScoped.BlahCount, Is.EqualTo(0));
        Assert.That(newScopedSingleton.BlahCount, Is.EqualTo(2));

        newScope.GetRequiredService<IMyDummyTestInterface>().Blah();

        Assert.That(newScopedTransient.BlahCount, Is.EqualTo(0));
        Assert.That(newScopedScoped.BlahCount, Is.EqualTo(1));
        Assert.That(newScopedSingleton.BlahCount, Is.EqualTo(3));

        T Get<T>(IServiceProvider? scope = null) where T : class
        {
            return scope.GetServices<IDecoratorInterfaceWrapper<IMyDummyTestInterface>>()
                .First(x => x.Value is T).Value as T;
        }
    }
}

public interface IMyDummyTestInterface
{
    void Blah();
}

public class MyBaseTestClass : IMyDummyTestInterface
{
    public int BlahCount { get; private set; }
    public void Blah()
    {
        BlahCount++;
    }
}

public class MyTransientTestClass : MyBaseTestClass
{
}

public class MyScopedTestClass : MyBaseTestClass
{
}

public class MySingletonTestClass : MyBaseTestClass
{
}