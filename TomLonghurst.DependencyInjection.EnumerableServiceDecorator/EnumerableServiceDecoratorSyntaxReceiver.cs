﻿using Microsoft.CodeAnalysis;
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

            var typeDeclared = methodSymbol.TypeArguments.FirstOrDefault() ?? throw new ArgumentException($"No Type provided for {nameof(DependencyInjectionExtensions.FlattenEnumerableToSingle)} call");

            var interfaceDeclarationSyntax = typeDeclared.DeclaringSyntaxReferences
                .Select(s => s.GetSyntax())
                .OfType<InterfaceDeclarationSyntax>()
                .FirstOrDefault()
                ?? throw new ArgumentException($"{typeDeclared} must be an interface in order to use in {nameof(DependencyInjectionExtensions.FlattenEnumerableToSingle)}");

            var methodDeclarationSyntaxes = interfaceDeclarationSyntax.Members
                .OfType<MethodDeclarationSyntax>()
                .ToList();

            var returnTypeExceptions = methodDeclarationSyntaxes
                .Where(m => m.ReturnType.ToString() is not ("void" or "Task" or "ValueTask"))
                .Select(m =>
                    new ArgumentException($"Only void or Task return types are supported. Cannot convert IEnumerable<{m.ReturnType}> to {m.ReturnType}")
                ).ToList();

            if (returnTypeExceptions.Any())
            {
                throw new AggregateException(returnTypeExceptions);
            }
            
            IdentifiedDecorators.Add(new IndentifiedDecorator
            {
                InterfaceType = typeDeclared,
                DependencyInjectionMethodCall = methodSymbol,
                MethodsInInterfaceSyntaxes = methodDeclarationSyntaxes,
            });
        }
    }
}
