using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using TomLonghurst.DependencyInjection.EnumerableServiceDecorator.Extensions;
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
            
            foreach (var methodSymbol in identifiedDecorator.MethodsInInterfaceSyntaxes)
            {
                var parametersWithType = methodSymbol.ParameterList.Parameters.Select(p =>
                    $"{context.Compilation.GetSemanticModel(methodSymbol.SyntaxTree).GetSymbolInfo(p.Type).Symbol} {p.Identifier}");

                var returnType = context.Compilation.GetSemanticModel(methodSymbol.SyntaxTree)
                    .GetSymbolInfo(methodSymbol.ReturnType).Symbol;
                var async = returnType.ToString() == "void" ? string.Empty : "async ";

                var parameterNames = methodSymbol.ParameterList.Parameters.Select(p => string.Join(", ", p.Identifier));
                
                codeWriter.WriteLine( $"public {async}{returnType} {methodSymbol.Identifier}{GetGenericType(methodSymbol)}({string.Join(", ", parametersWithType)})");
                codeWriter.WriteLine("{");
                GenerateBody(codeWriter, methodSymbol, parameterNames);
                codeWriter.WriteLine("}");
                codeWriter.WriteLine();
            }
            
            codeWriter.WriteLine("}");
            codeWriter.WriteLine("}");
            codeWriter.WriteLine();
        }

        return codeWriter.ToString();
    }

    private static void GenerateBody(TextWriter codeWriter, MethodDeclarationSyntax methodSymbol, IEnumerable<string> parameterNames)
    {
        if (methodSymbol.ReturnType.ToString() == "void")
        {
            codeWriter.WriteLine("foreach (var wrapper in _wrappers)");
            codeWriter.WriteLine("{");
            codeWriter.WriteLine($"wrapper.Value.{methodSymbol.Identifier}({string.Join(", ", parameterNames)});");
            codeWriter.WriteLine("}");
        }
        else
        {
            codeWriter.WriteLine($"foreach (var task in _wrappers.Select(wrapper => wrapper.Value.{methodSymbol.Identifier}({string.Join(", ", parameterNames)})))");
            codeWriter.WriteLine("{");
            codeWriter.WriteLine("await task;");
            codeWriter.WriteLine("}");
        }
    }

    private string GetGenericType(MethodDeclarationSyntax method)
    {
        if (method.TypeParameterList?.Parameters.Any() != true)
        {
            return string.Empty;
        }

        var genericTypes = method.TypeParameterList.Parameters.Select(x => x.Identifier);
        return $"<{string.Join(", ", genericTypes)}>";
    }
}