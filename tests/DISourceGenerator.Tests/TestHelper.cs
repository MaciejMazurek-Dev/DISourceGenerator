using DISourceGenerator.Library;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DISourceGenerator.Tests
{
    public static class TestHelper
    {
        public static Task Verify(string sourceCode)
        {
            // Parse source code into syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            // Compile syntax tree
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "SourceGeneratorTest", 
                syntaxTrees: new[] { syntaxTree });

            // Create instance of source generator
            SourceGenerator generator = new();

            // Run generator over compiled syntax tree
            GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(generator);
            generatorDriver = generatorDriver.RunGenerators(compilation);

            // Run snapshot test on source generator output
            return Verifier.Verify(generatorDriver);
        }
    }
}
