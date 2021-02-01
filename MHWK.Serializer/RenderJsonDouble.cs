using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MHWK.Serializer
{
    internal sealed class RenderJsonDouble : IRender
    {
        public bool Renders(ParameterSyntax parameter)
        {
            return parameter.Type.IsKind(SyntaxKind.PredefinedType)
                   && "double" == parameter.Type.ToString();
        }

        public string RenderDeserialization(ParameterSyntax parameter)
        {
            return $@"
                if (reader.GetString() == ""{parameter.Identifier}"")
                {{
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.Number) throw new ArgumentException(""Expected number for {parameter.Identifier}"");
                    {parameter.Identifier} = reader.GetDouble();
                    break;
                }}";
        }
    }
}