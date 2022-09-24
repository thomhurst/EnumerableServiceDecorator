# EnumerableServiceDecorator
Call all IEnumerable&lt;T> by calling a single injected T

## Installation
via Nuget: TomLonghurst.DependencyInjection.EnumerableServiceDecorator

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
