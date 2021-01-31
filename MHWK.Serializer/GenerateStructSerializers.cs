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
    public sealed class GenerateStructSerializers : ISourceGenerator
    {
        private static readonly IRender Render = new RenderJson();

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var attributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(SerializableAttribute).FullName);

            var classWithAttributes = context.Compilation.SyntaxTrees
                .Where(st => st.GetRoot()
                    .DescendantNodes()
                    .OfType<StructDeclarationSyntax>()
                    .Any(p => p.DescendantNodes().OfType<AttributeSyntax>().Any()));

            foreach (SyntaxTree tree in classWithAttributes)
            {
                var model = context.Compilation.GetSemanticModel(tree);

                foreach (var @struct in tree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<StructDeclarationSyntax>()
                    .Where(cd => cd.DescendantNodes().OfType<AttributeSyntax>().Any()))
                {
                    var nodes = @struct
                        .DescendantNodes()
                        .OfType<AttributeSyntax>()
                        .FirstOrDefault(a => a.DescendantTokens().Any(dt =>
                            dt.IsKind(SyntaxKind.IdentifierToken) && model.GetTypeInfo(dt.Parent).Type.Name ==
                            attributeSymbol.Name))
                        ?.DescendantTokens()
                        ?.Where(dt => dt.IsKind(SyntaxKind.IdentifierToken))
                        ?.ToList();

                    if (nodes == null)
                    {
                        continue;
                    }

                    var name = @struct.Identifier.ToString();
                    var ns = @struct
                        .Ancestors()
                        .OfType<NamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString() ?? throw new ArgumentException($"No namespace for {name}");
                    var constructor = @struct
                        .ChildNodes()
                        .OfType<ConstructorDeclarationSyntax>()
                        .OrderByDescending(c => c.ParameterList.ChildNodes().Count())
                        .FirstOrDefault() ?? throw new ArgumentException($"No constructor for {name}");
                    var parameters = constructor.ParameterList.ChildNodes().OfType<ParameterSyntax>();

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
            if (reader.TokenType != JsonTokenType.StartObject) throw new ArgumentException(""Expected start of object"");

            {string.Join(
                "\n",
                parameters.Select(parameter => $"var {parameter.Identifier} = default({parameter.Type?.ToString()});")
            )}

            while (reader.Read())
            {{
                switch (reader.TokenType)
                {{
                    default:
                        break;
                    case JsonTokenType.EndObject:
                        goto after;
                        break;
                    case JsonTokenType.PropertyName:
                        {string.Join(
                "\n", parameters
                    .Select(parameter => Render.Renders(parameter)
                        ? Render.RenderDeserialization(parameter)
                        : ""
                    ))}
                        break;
                }}
            }}

            after:

            return new {name}(
                {string.Join(", ", parameters.Select(parameter => parameter.Identifier.ToString()))}
            );
        }}
    }}
}}
";
            
                    context.AddSource(
                        $"{@struct.Identifier}Serializer",
                        SourceText.From(generated, Encoding.UTF8)
                    );
                }
            }
        }
    }
}