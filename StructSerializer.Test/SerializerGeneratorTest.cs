using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace StructSerializer.Test
{
    public class SerializerGeneratorTest
    {
        [Fact]
        public void Generate()
        {
            var inputCompilation = CreateCompilation(@"
using System;

namespace StructSerializer.Test
{
    public static class Program
    {
        public static void Main()
        {
        }
    }

    [Serializable]
    public readonly struct Person
    {
        public readonly string Name;

        public readonly Address Address;

        public Person(string name, Address address)
        {
            Name = name;
            Address = address;
        }
    }

    [Serializable]
    public readonly struct Address
    {
        public readonly string Street;

        public readonly int HouseNumber;

        public Address(string street, int houseNumber)
        {
            Street = street;
            HouseNumber = houseNumber;
        }
    }
}
");
            var generator = new SerializerGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the generation pass
            // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            // We can now assert things about the resulting compilation:
            Debug.Assert(diagnostics.IsEmpty); // there were no diagnostics created by the generators
            // Debug.Assert(outputCompilation.SyntaxTrees.Count() == 2); // we have two syntax trees, the original 'user' provided one, and the one added by the generator

            // Or we can look at the results directly:
            GeneratorDriverRunResult runResult = driver.GetRunResult();

            // The runResult contains the combined results of all generators passed to the driver
            // Debug.Assert(runResult.GeneratedTrees.Length == 1);
            Debug.Assert(runResult.Diagnostics.IsEmpty);

            // Or you can access the individual results on a by-generator basis
            // GeneratorRunResult generatorResult = runResult.Results[0];
            // Debug.Assert(generatorResult.Generator == generator);
            // Debug.Assert(generatorResult.Diagnostics.IsEmpty);
            // Debug.Assert(generatorResult.GeneratedSources.Length == 1);
            // Debug.Assert(generatorResult.Exception is null);
        }
        
        private static Compilation CreateCompilation(string source)
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