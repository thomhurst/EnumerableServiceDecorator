# EnumerableServiceDecorator
Call all IEnumerable&lt;T> by calling a single injected T

[![nuget](https://img.shields.io/nuget/v/TomLonghurst.DependencyInjection.EnumerableServiceDecorator.svg)](https://www.nuget.org/packages/TomLonghurst.DependencyInjection.EnumerableServiceDecorator/)
<!-- ![Nuget](https://img.shields.io/nuget/dt/TomLonghurst.DependencyInjection.EnumerableServiceDecorator) -->

## Support

If you like this library, consider buying me a coffee.

<a href="https://www.buymeacoffee.com/tomhurst" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>

## Usage

1. Register multiple implementations of your interfaces(s)

```csharp
services.AddSingleton<IMyInterface, MyImplementation1>()
    .AddSingleton<IMyInterface, MyImplementation2>()
    .AddSingleton<IMyInterface, MyImplementation3>();
```

2. After all your dependencies have been added (so at the END - the order matters!) call `FlattenEnumerableToSingle<T>`

```csharp
        services.FlattenEnumerableToSingle<IMyInterface>()
```

3. Inject just one `T` into your class

```csharp
public class MyWorker
{
  private readonly IMyInterface _myInterface;
  
  public MyWorker(IMyInterface myInterface)
  {
    _myInterface = myInterface;
  }
}
```

4. Call a method on your interface. It'll automagically call the same method in all of your registered classes

```csharp
_myInterface.DoSomething();
await _myInterface.DoSomethingElseAsync();
```

The above will essentially do:
```csharp
MyImplementation1.DoSomething();
MyImplementation2.DoSomething();
MyImplementation3.DoSomething();

await MyImplementation1.DoSomethingElseAsync();
await MyImplementation2.DoSomethingElseAsync();
await MyImplementation3.DoSomethingElseAsync();
```

5. Enjoy!

## Caveats

This doesn't support Properties in your interfaces. There's no way to return multiple implementations of a property in just a single property.

The same with methods with return types. We can't turn an IEnumerable<T> into a T. 
    
So we can only support interfaces with methods that return void or Task.
