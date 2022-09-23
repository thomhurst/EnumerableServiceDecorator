using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator;

public class IndentifiedDecorator
{
    public IMethodSymbol DependencyInjectionMethodCall { get; set; }
        
    public ITypeSymbol InterfaceType { get; set; } 
        
    public IEnumerable<MethodDeclarationSyntax> MethodsInInterfaceSyntaxes { get; set; }
    
    public IEnumerable<IMethodSymbol> MethodsInInterfaceSymbols { get; set; }
}