using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace StructSerializer
{
    [Generator]
    public class SerializerGenerator : ISourceGenerator
    {
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
                var semanticModel = context.Compilation.GetSemanticModel(tree);

                foreach (var declaredClass in tree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<StructDeclarationSyntax>()
                    .Where(cd => cd.DescendantNodes().OfType<AttributeSyntax>().Any()))
                {
                    var nodes = declaredClass
                        .DescendantNodes()
                        .OfType<AttributeSyntax>()
                        .FirstOrDefault(a => a.DescendantTokens().Any(dt =>
                            dt.IsKind(SyntaxKind.IdentifierToken) && semanticModel.GetTypeInfo(dt.Parent).Type.Name ==
                            attributeSymbol.Name))
                        ?.DescendantTokens()
                        ?.Where(dt => dt.IsKind(SyntaxKind.IdentifierToken))
                        ?.ToList();

                    if (nodes == null)
                    {
                        continue;
                    }

                    var relatedClass = semanticModel.GetTypeInfo(nodes.Last().Parent);
                    var generatedClass = GenerateClass(declaredClass);

                    context.AddSource($"{declaredClass.Identifier}Serializer",
                        SourceText.From(generatedClass, Encoding.UTF8));
                }
            }
        }

        private string GenerateClass(StructDeclarationSyntax @struct)
        {
            var className = @struct.Identifier.ToString();
            var classNamespace = @struct.Ancestors().OfType<NamespaceDeclarationSyntax>().First().Name.ToString();
            var constructor = @struct.ChildNodes().OfType<ConstructorDeclarationSyntax>()
                .OrderByDescending(constructor => constructor.ParameterList.ChildNodes().Count())
                .FirstOrDefault() ?? throw new ArgumentException($"No constructor for {className}");
            var parameters = constructor.ParameterList.ChildNodes().OfType<ParameterSyntax>();

            return $@"
using StructSerializer;
using System;
using System.Text.Json;

namespace {classNamespace}
{{
    public sealed class {className}Serializer : Serializer<{className}>
    {{
        public static readonly {className}Serializer Instance = new {className}Serializer();

        private {className}Serializer()
        {{
        }}

        public override {className} Deserialize(Utf8JsonReader reader) 
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
                    //default: throw new ArgumentException(""Unexpected json token"");
                    default: continue;
                    case JsonTokenType.EndObject: goto after;
                    case JsonTokenType.PropertyName:
                        {string.Join("\n", parameters.Select(argument => {
                            if (argument.Type.IsKind(SyntaxKind.PredefinedType) && "string" == argument.Type.ToString()) {
                                return $@"
                                    if (reader.GetString() == ""{argument.Identifier}"")
                                    {{
                                        reader.Read();
                                        if (reader.TokenType != JsonTokenType.String) throw new ArgumentException(""Expected string for {argument.Identifier}"");
                                        {argument.Identifier} = reader.GetString();
                                    }}
                                ";
                            }

                            if (argument.Type.IsKind(SyntaxKind.PredefinedType) && "int" == argument.Type.ToString()) {
                                return $@"
                                    if (reader.GetString() == ""{argument.Identifier}"")
                                    {{
                                        reader.Read();
                                        if (reader.TokenType != JsonTokenType.Number) throw new ArgumentException(""Expected number for {argument.Identifier}"");
                                        {argument.Identifier} = reader.GetInt32();
                                    }}
                                ";
                            }
                            
                            if (argument.Type.IsKind(SyntaxKind.IdentifierName)) {
                                return $@"
                                    if (reader.GetString() == ""{argument.Identifier}"")
                                    {{
                                        {argument.Identifier} = {argument.Type}Serializer.Instance.Deserialize(reader);
                                    }}
                                ";
                            }

                            return "";
                        }))}
                        break;
                }}
            }}

            after:

            return new {className}(
                {string.Join(", ", parameters.Select(parameter => parameter.Identifier.ToString()))}
            );
        }}
    }}
}}
";
        }
    }
}