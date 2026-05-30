using Microsoft.CodeAnalysis;

namespace DISourceGenerator.Library;

[Generator(LanguageNames.CSharp)]
public class DISourceGenerator : IIncrementalGenerator
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
    }
}
