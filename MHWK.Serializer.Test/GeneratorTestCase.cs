using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace MHWK.Serializer.Test
{
    public abstract class GeneratorTestCase
    {
        public abstract ISourceGenerator Generator { get; }
        
        public abstract string Source { get; }

        [Fact]
        public void Test()
        {
            var inputCompilation = CreateCompilation($@"
using System;

namespace MHWK.Serializer.Test
{{
    public static class Program
    {{
        public static void Main()
        {{
        }}
    }}
}}

{Source}
");
            
            Assert(CSharpGeneratorDriver
                .Create(Generator)
                .RunGeneratorsAndUpdateCompilation(
                    inputCompilation,
                    out var outputCompilation,
                    out var diagnostics
                    )
                .GetRunResult()
            );
        }

        public abstract void Assert(GeneratorDriverRunResult result);
        
        protected static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(Object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(SerializableAttribute).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Serializer<>).GetTypeInfo().Assembly.Location),
                },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}