namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.Example;

public interface MyInterface2
{
    void MyMethod(MyDependency myDependency);
    void MyMethod2(string blah);
}

public class MyClass4 : MyInterface2
{
    public void MyMethod(MyDependency myDependency)
    {
        Console.WriteLine($"Hi, from {GetType().FullName}");
    }

    public void MyMethod2(string blah)
    {
        Console.WriteLine($"Hi again, from {GetType().FullName}");
    }
}

public class MyClass5 : MyInterface2
{
    public void MyMethod(MyDependency myDependency)
    {
        Console.WriteLine($"Hi, from {GetType().FullName}");
    }
    
    public void MyMethod2(string blah)
    {
        Console.WriteLine($"Hi again, from {GetType().FullName}");
    }
}

public class MyClass6 : MyInterface2
{
    public void MyMethod(MyDependency myDependency)
    {
        Console.WriteLine($"Hi, from {GetType().FullName}");
    }
    
    public void MyMethod2(string blah)
    {
        Console.WriteLine($"Hi again, from {GetType().FullName}");
    }
}