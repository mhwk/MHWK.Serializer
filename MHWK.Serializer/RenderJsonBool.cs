using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MHWK.Serializer
{
    public sealed class RenderJsonBool : IRender
    {
        public bool Renders(ParameterSyntax parameter)
        {
            return parameter.Type.IsKind(SyntaxKind.PredefinedType)
                   && "bool" == parameter.Type.ToString();
        }

        public string RenderDeserialization(ParameterSyntax parameter)
        {
            return $@"
                if (reader.GetString() == ""{parameter.Identifier}"")
                {{
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False) throw new ArgumentException(""Expected boolean for {parameter.Identifier}"");
                    {parameter.Identifier} = reader.GetBoolean();
                    break;
                }}";
        }
    }
}