using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MHWK.Serializer
{
    internal sealed class RenderJsonString : IRender
    {
        public bool Renders(ParameterSyntax parameter)
        {
            return parameter.Type.IsKind(SyntaxKind.PredefinedType) && "string" == parameter.Type.ToString();
        }

        public string RenderDeserialization(ParameterSyntax parameter)
        {
            return $@"
                if (reader.GetString() == ""{parameter.Identifier}"")
                {{
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.String) throw new ArgumentException(""Expected string for {parameter.Identifier}"");
                    {parameter.Identifier} = reader.GetString();
                }}";
        }
    }
}