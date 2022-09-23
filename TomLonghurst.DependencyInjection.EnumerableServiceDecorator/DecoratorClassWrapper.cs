namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator;

internal class DecoratorClassWrapper<T> : IDecoratorInterfaceWrapper<T>
{
    public T Value { get; }

    public DecoratorClassWrapper(T value)
    {
        Value = value;
    }
}