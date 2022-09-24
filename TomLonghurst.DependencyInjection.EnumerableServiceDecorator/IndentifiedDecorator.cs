using Microsoft.CodeAnalysis;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator;

public class IndentifiedDecorator
{
    public IMethodSymbol DependencyInjectionMethodCall { get; set; }
        
    public ITypeSymbol InterfaceType { get; set; }

    public IReadOnlyList<IMethodSymbol> MethodsInInterface { get; set; }
}