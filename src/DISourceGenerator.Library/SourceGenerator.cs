using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;

namespace DISourceGenerator.Library;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        // Add marker attribute 
        initContext.RegisterPostInitializationOutput(callback =>
        {
            const string markerAttribute = @"   
                                                [AttributeUsage(AttributeTargets.Class)]
                                                public class MarkerAttribute : Attribute
                                                {
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
        var markers = initContext.SyntaxProvider.CreateSyntaxProvider(
            predicate: (syntaxNode, _) =>
            {
                string name = string.Empty;
                if(syntaxNode is not AttributeSyntax attribute)
                {
                    return false;
                }
                if(attribute.Name is SimpleNameSyntax sns)
                {
                    name = sns.Identifier.Text;
                }
                else if(attribute.Name is QualifiedNameSyntax qns)
                {
                    name = qns.Right.Identifier.Text;
                }

                if(name == "Marker")
                {
                    return true;
                }
                return false;
            },
            transform: (syntaxContext, _) =>
            {
                if(syntaxContext.Node.Parent is not ClassDeclarationSyntax classSyntax)
                {
                    return null;
                }
                var classSemantic = syntaxContext.SemanticModel.GetDeclaredSymbol(classSyntax);

                return classSemantic?.GetAttributes().Any();
            });

        // Generate source code
    }

}
