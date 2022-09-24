using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator
{
    internal class EnumerableServiceDecoratorSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<IndentifiedDecorator> IdentifiedDecorators { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if(context.Node is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                Process(context, invocationExpressionSyntax);
            }
        }

        private void Process(GeneratorSyntaxContext context, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol;

            if (symbol is not IMethodSymbol methodSymbol)
            {
                return;
            }

            if (methodSymbol.ContainingType.ToDisplayString(SymbolDisplayFormats.NamespaceAndType) != typeof(DependencyInjectionExtensions).FullName)
            {
                return;
            }

            if (methodSymbol.Name != nameof(DependencyInjectionExtensions.FlattenEnumerableToSingle))
            {
                return;
            }

            var interfaceSymbol = methodSymbol.TypeArguments.FirstOrDefault() ?? throw new ArgumentException($"No Type provided for {nameof(DependencyInjectionExtensions.FlattenEnumerableToSingle)} call");

            var interfaceSymbols = interfaceSymbol.GetMembers();

            if (interfaceSymbols.OfType<IPropertySymbol>().Any())
            {
                throw new ArgumentException("Properties are not supported. Cannot return multiple property implementations from a single flattened class.");
            }

            var interfaceMethodSymbols = interfaceSymbols
                .OfType<IMethodSymbol>()
                .ToList();

            var allowedTypes = new[]
            {
                typeof(Task).FullName,
                typeof(void).FullName,
                typeof(ValueTask).FullName
            };

            var returnTypeExceptions = interfaceMethodSymbols
                .Where(m => !allowedTypes.Contains(m.ReturnType.ToDisplayString(SymbolDisplayFormats.NamespaceAndType)))
                .Select(m =>
                    new ArgumentException($"Only void or Task return types are supported. Cannot convert IEnumerable<{m.ReturnType}> to {m.ReturnType}")
                ).ToList();

            if (returnTypeExceptions.Any())
            {
                throw new AggregateException(returnTypeExceptions);
            }
            
            IdentifiedDecorators.Add(new IndentifiedDecorator
            {
                InterfaceType = interfaceSymbol,
                DependencyInjectionMethodCall = methodSymbol,
                MethodsInInterface = interfaceMethodSymbols,
            });
        }
    }
}
