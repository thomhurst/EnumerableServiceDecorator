namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator;

public interface IDecoratorInterfaceWrapper<T>
{
    T Value { get; }
}