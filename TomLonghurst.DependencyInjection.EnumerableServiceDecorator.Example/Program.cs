// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.Example;

public class Program
{
    public static void Main(string[] args)
    {
        var services = new ServiceCollection()

        .AddSingleton<MyInterface, MyClass1>()
        .AddSingleton<MyInterface, MyClass2>()
        .AddSingleton<MyInterface, MyClass3>()
        .FlattenEnumerableToSingle<MyInterface>()
        
        .AddSingleton<MyInterface2, MyClass4>()
        .AddSingleton<MyInterface2, MyClass5>()
        .AddSingleton<MyInterface2, MyClass6>()
        .FlattenEnumerableToSingle<MyInterface2>()
        
        .AddSingleton<MyInterface3, MyClass7>()
        .AddSingleton<MyInterface3, MyClass8>()
        .AddSingleton<MyInterface3, MyClass9>()
        .FlattenEnumerableToSingle<MyInterface3>()
        
        .BuildServiceProvider();
    }
}