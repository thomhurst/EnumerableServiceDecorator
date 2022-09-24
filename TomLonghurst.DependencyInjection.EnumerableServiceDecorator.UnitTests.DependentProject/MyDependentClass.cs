namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.UnitTests.DependentProject;

public class MyDependentClass : IMyDependentInterface
{
    public void MyDependentMethod(ref int result)
    {
        result = 123;
    }
}