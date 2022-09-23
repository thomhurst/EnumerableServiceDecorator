using Microsoft.CodeAnalysis;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator;

public static class SymbolDisplayFormats
{
    public static readonly SymbolDisplayFormat NamespaceAndType =
        new(
            SymbolDisplayGlobalNamespaceStyle.Omitted,
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            SymbolDisplayGenericsOptions.IncludeTypeParameters
        );
}