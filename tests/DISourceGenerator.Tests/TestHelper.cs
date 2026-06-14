using DISourceGenerator.Library;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: CaptureConsole]
namespace DISourceGenerator.Tests
{

    public static class TestHelper
    {
        [ModuleInitializer]
        public static void Init() => VerifySourceGenerators.Initialize();


        public static Task Verify(string sourceCode)
        {
            // Parse source code into syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            // Get reference to .NET runtime library 
            IEnumerable <PortableExecutableReference> references = new[]
                {
                    MetadataReference.CreateFromFile(typeof(System.Object).Assembly.Location),
                };

            // Compile syntax tree
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "SourceGeneratorTest", 
                syntaxTrees: new[] { syntaxTree },
                references: references);

            // Display compilation errors
            ImmutableArray<Diagnostic> diagnostic = compilation.GetDiagnostics();
            foreach(var diag in diagnostic)
            {
                Console.WriteLine("Compilation error: " + diag.GetMessage());
            }

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
