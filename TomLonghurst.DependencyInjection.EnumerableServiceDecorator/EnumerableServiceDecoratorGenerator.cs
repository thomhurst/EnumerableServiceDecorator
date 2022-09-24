using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TomLonghurst.DependencyInjection.EnumerableServiceDecorator.Helpers;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator;

[Generator]
public class EnumerableServiceDecoratorGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUG
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            //System.Diagnostics.Debugger.Launch();
        }
#endif
        
        context.RegisterForSyntaxNotifications(() => new EnumerableServiceDecoratorSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not EnumerableServiceDecoratorSyntaxReceiver syntaxReciever)
        {
            return;
        }

        var source = GenerateSource(context, syntaxReciever);

        context.AddSource("EnumerableServiceDecorator.generated", SourceText.From(source, Encoding.UTF8));
    }

    private string GenerateSource(GeneratorExecutionContext context, EnumerableServiceDecoratorSyntaxReceiver syntaxReciever)
    {
        var codeWriter = new CodeGenerationTextWriter();
        
        codeWriter.WriteLine(context.GetUsingStatementsForTypes(
            syntaxReciever.IdentifiedDecorators.Select(d => d.InterfaceType),
            typeof(DependencyInjectionExtensions),
            typeof(DecoratorClassWrapper<>),
            typeof(IDecoratorInterfaceWrapper<>),
            typeof(IEnumerable<>),
            typeof(Enumerable),
            typeof(string),
            typeof(Task),
            typeof(Task<>)
        ));   
        codeWriter.WriteLine();

        
        foreach (var identifiedDecorator in syntaxReciever.IdentifiedDecorators.DistinctBy(d => d.InterfaceType))
        {
            var typeSymbol = identifiedDecorator.InterfaceType;
            
            var interfaceName = typeSymbol.ToDisplayString(SymbolDisplayFormats.NamespaceAndType);
            var className = $"{interfaceName.Replace(".", string.Empty)}EnumerableServiceDecorator";

            var decoratorName = $"{nameof(IDecoratorInterfaceWrapper<object>)}<{interfaceName}>";

            
            codeWriter.WriteLine($"namespace {typeSymbol.ContainingNamespace}");
            codeWriter.WriteLine("{");
            codeWriter.WriteLine($"public class {className} : {interfaceName}");
            codeWriter.WriteLine("{");
            codeWriter.WriteLine($"private readonly IEnumerable<{decoratorName}> _wrappers;");
            codeWriter.WriteLine();
            codeWriter.WriteLine($"public {className}(IEnumerable<{decoratorName}> wrappers)");
            codeWriter.WriteLine("{");
            codeWriter.WriteLine("_wrappers = wrappers;");
            codeWriter.WriteLine("}");
            codeWriter.WriteLine();
            
            foreach (var methodSymbol in identifiedDecorator.MethodsInInterface)
            {
                var parametersWithType = methodSymbol.Parameters.Select(p =>
                    $"{GetRef(p.RefKind)} {string.Join(" ", p.RefCustomModifiers)} {string.Join(" ", p.CustomModifiers)} {p.Type.ToDisplayString(SymbolDisplayFormats.NamespaceAndType)} {p.Name}".Trim());

                var returnType = methodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormats.NamespaceAndType) ;
                
                if (returnType == "System.Void")
                {
                    returnType = "void";
                }

                var async = methodSymbol.ReturnType.SpecialType == SpecialType.System_Void ? string.Empty : "async ";

                var parameterNames = methodSymbol.Parameters.Select(p => string.Join(", ", p.Name));
                
                codeWriter.WriteLine( $"public {async}{returnType} {methodSymbol.Name}{GetGenericType(methodSymbol)}({string.Join(", ", parametersWithType)})");
                codeWriter.WriteLine("{");
                GenerateBody(codeWriter, methodSymbol);
                codeWriter.WriteLine("}");
                codeWriter.WriteLine();
            }
            
            codeWriter.WriteLine("}");
            codeWriter.WriteLine("}");
            codeWriter.WriteLine();
        }

        return codeWriter.ToString();
    }

    private static void GenerateBody(TextWriter codeWriter, IMethodSymbol methodSymbol)
    {
        var parameters = methodSymbol.Parameters.Select(
            p => $"{GetRef(p.RefKind)} {p.Name}".Trim()
        );
        
        if (methodSymbol.ReturnType.ToString() == "void")
        {
            codeWriter.WriteLine("foreach (var wrapper in _wrappers)");
            codeWriter.WriteLine("{");
            codeWriter.WriteLine($"wrapper.Value.{methodSymbol.Name}({string.Join(", ", parameters)});");
            codeWriter.WriteLine("}");
        }
        else
        {
            codeWriter.WriteLine($"foreach (var task in _wrappers.Select(wrapper => wrapper.Value.{methodSymbol.Name}({string.Join(", ", parameters)})))");
            codeWriter.WriteLine("{");
            codeWriter.WriteLine("await task;");
            codeWriter.WriteLine("}");
        }
    }

    private static string GetGenericType(IMethodSymbol method)
    {
        if (method.TypeParameters.Any() != true)
        {
            return string.Empty;
        }

        var genericTypes = method.TypeParameters.Select(x => x.Name);
        return $"<{string.Join(", ", genericTypes)}>";
    }

    private static string GetRef(RefKind refKind)
    {
        return refKind switch
        {
            RefKind.None => string.Empty,
            RefKind.Ref => "ref",
            RefKind.Out => "out",
            RefKind.In => "in",
            _ => throw new ArgumentOutOfRangeException(nameof(refKind), refKind, null)
        };
    }
}