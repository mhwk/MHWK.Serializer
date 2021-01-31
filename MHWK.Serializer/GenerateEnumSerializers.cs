using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MHWK.Serializer
{
    [Generator]
    public class GenerateEnumSerializers : ISourceGenerator
    {
        private static readonly IRender Render = new RenderJson();

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var attributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(SerializableAttribute).FullName);

            foreach (SyntaxTree tree in context.Compilation.SyntaxTrees)
            {
                var model = context.Compilation.GetSemanticModel(tree);

                foreach (var @enum in tree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<EnumDeclarationSyntax>())
                {
                    if (!@enum
                        .DescendantNodes()
                        .OfType<AttributeSyntax>()
                        .FirstOrDefault(a => a.DescendantTokens().Any(dt =>
                            dt.IsKind(SyntaxKind.IdentifierToken) && model.GetTypeInfo(dt.Parent).Type.Name ==
                            attributeSymbol.Name))
                        ?.DescendantTokens()
                        ?.Any(dt => dt.IsKind(SyntaxKind.IdentifierToken)) ?? false)
                    {
                        continue;
                    }
                    
                    var name = @enum.Identifier.ToString();
                    var ns = @enum
                        .Ancestors()
                        .OfType<NamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString() ?? throw new ArgumentException($"No namespace for {name}");

                    var generated = $@"
using MHWK.Serializer;
using System;
using System.Text.Json;

namespace {ns}
{{
    public sealed class {name}Serializer : Serializer<{name}>
    {{
        public static readonly {name}Serializer Instance = new {name}Serializer();

        private {name}Serializer()
        {{
        }}

        public override {name} Deserialize(ref Utf8JsonReader reader) 
        {{
            reader.Read();
            
            if (reader.TokenType != JsonTokenType.String) throw new ArgumentException($""Expected string for enum {{typeof({name}).FullName}}"");

            return ({name}) Enum.Parse(typeof({name}), reader.GetString(), true);
        }}
    }}
}}";
                    
                    context.AddSource(
                        $"{@enum.Identifier}Serializer",
                        SourceText.From(generated, Encoding.UTF8)
                    );
                }
            }
        }
    }
}