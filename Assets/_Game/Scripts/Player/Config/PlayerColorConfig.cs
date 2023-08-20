using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerColorConfig", menuName = "ScriptableObjects/Configs/PlayerColorConfig", order = 1)]
public class PlayerColorConfig : ScriptableObject
{
    [SerializeField] private List<Color> playerColors;

    public List<Color> PlayerColors => playerColors;

    public int GetIndexFromColor(Color color)
    {
        for (var index = 0; index < playerColors.Count; index++)
        {
            var col = playerColors[index];
            if (color.Equals(col))
            {
                return index;
            }
        }

        return -1;

        throw new NotSupportedException($"couldn't find color {color.ToString()}");
    }
}
