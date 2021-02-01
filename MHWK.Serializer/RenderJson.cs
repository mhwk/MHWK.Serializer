using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MHWK.Serializer
{
    internal sealed class RenderJson : IRender
    {
        private static readonly IRender[] Renderers = {
            new RenderJsonBool(),
            new RenderJsonDouble(),
            new RenderJsonFloat(),
            new RenderJsonInt(),
            new RenderJsonObject(),
            new RenderJsonString(),
        };
        
        public bool Renders(ParameterSyntax parameter)
        {
            return Renderers.Any(renderer => renderer.Renders(parameter));
        }

        public string RenderDeserialization(ParameterSyntax parameter)
        {
            return Renderers.First(renderer => renderer.Renders(parameter)).RenderDeserialization(parameter);
        }
    }
}