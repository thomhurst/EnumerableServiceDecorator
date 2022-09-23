using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TomLonghurst.DependencyInjection.EnumerableServiceDecorator;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.UnitTests;

public class Tests
{
    [Test]
    public void VoidTest()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IMyTestInterface, MyTestClass>()
            .AddSingleton<IMyTestInterface, MyTestClass2>()
            .AddSingleton<IMyTestInterface, MyTestClass3>()
            .FlattenEnumerableToSingle<IMyTestInterface>()
            .BuildServiceProvider();
        
        var stringBuilder = new StringBuilder();

        var myInterface = serviceProvider.GetRequiredService<IMyTestInterface>();
        
        Assert.That(myInterface.GetType(), Is.EqualTo(typeof(TomLonghurstDependencyInjectionEnumerableServiceDecoratorUnitTestsIMyTestInterfaceEnumerableServiceDecorator)));
        
        myInterface.Blah(str => stringBuilder.Append(str + " "));
        
        Assert.That(stringBuilder.ToString(), Is.EqualTo("MyTestClass MyTestClass2 MyTestClass3 "));
    }

    [Test]
    public async Task AsyncTest()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IMyAsyncTestInterface, MyAsyncTestClass>()
            .AddSingleton<IMyAsyncTestInterface, MyAsyncTestClass2>()
            .AddSingleton<IMyAsyncTestInterface, MyAsyncTestClass3>()
            .FlattenEnumerableToSingle<IMyAsyncTestInterface>()
            .BuildServiceProvider();
        
        var stringBuilder = new StringBuilder();

        var myInterface = serviceProvider.GetRequiredService<IMyAsyncTestInterface>();
        
        Assert.That(myInterface.GetType(), Is.EqualTo(typeof(TomLonghurstDependencyInjectionEnumerableServiceDecoratorUnitTestsIMyAsyncTestInterfaceEnumerableServiceDecorator)));

        var task = myInterface.BlahAsync(async str =>
        {
            await Task.Delay(1000);
            stringBuilder.Append(str + " ");
        });

        // We didn't await so no time for the StringBuilder to be called
        
        Assert.That(stringBuilder.ToString(), Is.EqualTo(""));

        await task;
        
        Assert.That(stringBuilder.ToString(), Is.EqualTo("MyAsyncTestClass MyAsyncTestClass2 MyAsyncTestClass3 "));
    }
    
    [Test]
    public async Task ValueTaskAsyncTest()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IMyValueTaskAsyncTestInterface, MyValueTaskAsyncTestClass>()
            .AddSingleton<IMyValueTaskAsyncTestInterface, MyValueTaskAsyncTestClass2>()
            .AddSingleton<IMyValueTaskAsyncTestInterface, MyValueTaskAsyncTestClass3>()
            .FlattenEnumerableToSingle<IMyValueTaskAsyncTestInterface>()
            .BuildServiceProvider();
        
        var stringBuilder = new StringBuilder();

        var myInterface = serviceProvider.GetRequiredService<IMyValueTaskAsyncTestInterface>();
        
        Assert.That(myInterface.GetType(), Is.EqualTo(typeof(TomLonghurstDependencyInjectionEnumerableServiceDecoratorUnitTestsIMyValueTaskAsyncTestInterfaceEnumerableServiceDecorator)));

        var task = myInterface.BlahAsync(async str =>
        {
            await Task.Delay(100);
            stringBuilder.Append(str + " ");
        });

        // We didn't await so no time for the StringBuilder to be called
        
        Assert.That(stringBuilder.ToString(), Is.EqualTo(""));

        await task;
        
        Assert.That(stringBuilder.ToString(), Is.EqualTo("MyValueTaskAsyncTestClass MyValueTaskAsyncTestClass2 MyValueTaskAsyncTestClass3 "));
    }
}
