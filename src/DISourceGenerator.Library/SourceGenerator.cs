using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DISourceGenerator.Library;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        // Add marker attribute 
        initContext.RegisterPostInitializationOutput(callback =>
        {
            const string markerAttribute = @"// auto-generated
                                                using System;
                                                namespace DISourceGenerator.Library
                                                {
                                                [AttributeUsage(System.AttributeTargets.Class)]
                                                public class MarkerAttribute : Attribute
                                                { }
                                                }
                                            ";
            callback.AddSource("MarkerAttribute.g.cs", markerAttribute);
        });


        // Add filters
        //
        // Predicate: 
        // Fires up everytime when change in source code is made
        // Transform:
        // Runs when predicate returns TRUE
        IncrementalValuesProvider<ClassToGenerate?> classesToGenerate = initContext.SyntaxProvider.CreateSyntaxProvider(
            predicate: (syntaxNode, _) => IsNodeForGeneration(syntaxNode),
            transform: (syntaxContext, _) => GetClassToGenerate(syntaxContext))
            .Where(c => c is not null);
            

        // Generate source code
        initContext.RegisterSourceOutput(classesToGenerate, (spc, source) =>
        {
                GenerateCode(spc, source);
        });
    }

    private static bool IsNodeForGeneration(SyntaxNode node)
    {
        string name = string.Empty;
        if (node is not AttributeSyntax attribute)
        {
            return false;
        }
        if (attribute.Name is SimpleNameSyntax sns)
        {
            name = sns.Identifier.Text;
        }
        else if (attribute.Name is QualifiedNameSyntax qns)
        {
            name = qns.Right.Identifier.Text;
        }

        if (name == "Marker")
        {
            return true;
        }
        return false;
    }

    private static ClassToGenerate? GetClassToGenerate(GeneratorSyntaxContext context)
    {
        if (context.Node.Parent?.Parent is not ClassDeclarationSyntax classSyntax)
        {
            return null;
        }

        var classData = context.SemanticModel.GetDeclaredSymbol(classSyntax);
        if(classData is not null)
        {
            return new ClassToGenerate(classData.Name);
        }
        return null;
    }

    private static void GenerateCode(SourceProductionContext spc, ClassToGenerate? classToGenerate)
    {
        if (classToGenerate is { } classData)
        {
            StringBuilder stringBuilder = new();

// Template for generating source code
//----------------------------------
            stringBuilder.Append(@"
// auto-generated
public partial ").Append(classData.Name).Append(@"
{
    // Rumpology
}");
//-----------------------------------

            spc.AddSource($"{classData.Name}.g.cs", stringBuilder.ToString());
         }
    }
}
