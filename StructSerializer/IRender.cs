using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StructSerializer
{
    public interface IRender
    {
        bool Renders(ParameterSyntax parameter);
        
        string RenderDeserialization(ParameterSyntax parameter);
    }
}