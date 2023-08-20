using System.Collections.Generic;

namespace MeshTools
{
    public class Shape
    {
        public bool IsRoot;
        public Border Outline;
        public List<Border> Holes;
    }
}