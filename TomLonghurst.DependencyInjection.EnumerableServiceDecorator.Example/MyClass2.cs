namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.Example;

public class MyClass2 : MyInterface
{
    public void MyMethod()
    {
        Console.WriteLine($"Hi, from {GetType().FullName}");
    }
}