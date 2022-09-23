namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.Example;

public class MyClass1 : MyInterface
{
    public void MyMethod()
    {
        Console.WriteLine($"Hi, from {GetType().FullName}");
    }
}