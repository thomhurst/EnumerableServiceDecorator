namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.Example;

public interface MyInterface3
{
    Task MyMethod(MyDependency myDependency, string blah, int blah2);
    Task MyMethod2(string blah, bool toggle, int number, string message);
}

public class MyClass7 : MyInterface3
{
    public Task MyMethod(MyDependency myDependency, string blah, int blah2)
    {
        Console.WriteLine($"Hi, from {GetType().FullName}");
        return Task.CompletedTask;
    }
    
    public Task MyMethod2(string blah, bool toggle, int number, string message)
    {
        Console.WriteLine($"Hi again, from {GetType().FullName}");
        return Task.CompletedTask;
    }
}

public class MyClass8 : MyInterface3
{
    public Task MyMethod(MyDependency myDependency, string blah, int blah2)
    {
        Console.WriteLine($"Hi, from {GetType().FullName}");
        return Task.CompletedTask;
    }
    
    public Task MyMethod2(string blah, bool toggle, int number, string message)
    {
        Console.WriteLine($"Hi again, from {GetType().FullName}");
        return Task.CompletedTask;
    }
}

public class MyClass9 : MyInterface3
{
    public Task MyMethod(MyDependency myDependency, string blah, int blah2)
    {
        Console.WriteLine($"Hi, from {GetType().FullName}");
        return Task.CompletedTask;
    }
    
    public Task MyMethod2(string blah, bool toggle, int number, string message)
    {
        Console.WriteLine($"Hi again, from {GetType().FullName}");
        return Task.CompletedTask;
    }
}