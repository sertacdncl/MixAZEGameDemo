using System.Collections.Generic;
using UnityEngine;

namespace MeshTools
{
    public class MeshGenerationContext
    {
        public ICollection<Vector2Int> CellPositions;
        public Vector2Int PlanePaddingSize;
        public float WallDepth;
        public CornerConfig Config;
        public List<Vector3[]> LineRendererPoints;

        public MeshGenerationContext(
            ICollection<Vector2Int> cellPositions,
            Vector2Int planePaddingSize,
            float wallDepth = 1f,
            CornerConfig config = default
            )
        {
            CellPositions = cellPositions;
            PlanePaddingSize = planePaddingSize;
            if(planePaddingSize.sqrMagnitude < 1)
                PlanePaddingSize = Vector2Int.one;
            WallDepth = wallDepth;
            Config = config;
            Config.ValidateConfig();
            LineRendererPoints = new List<Vector3[]>();
        }
    }
}
