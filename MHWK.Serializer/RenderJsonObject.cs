using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MHWK.Serializer
{
    internal sealed class RenderJsonObject : IRender
    {
        public bool Renders(ParameterSyntax parameter)
        {
            return parameter.Type.IsKind(SyntaxKind.IdentifierName);
        }

        public string RenderDeserialization(ParameterSyntax parameter)
        {
            return $@"
                if (reader.GetString() == ""{parameter.Identifier}"")
                {{
                    {parameter.Identifier} = {parameter.Type}Serializer.Instance.Deserialize(ref reader);
                    break;
                }}";
        }
    }
}