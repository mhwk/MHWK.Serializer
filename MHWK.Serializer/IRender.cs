using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MHWK.Serializer
{
    public interface IRender
    {
        bool Renders(ParameterSyntax parameter);
        
        string RenderDeserialization(ParameterSyntax parameter);
    }
}