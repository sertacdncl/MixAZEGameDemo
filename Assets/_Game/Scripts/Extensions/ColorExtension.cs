using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class ColorExtension
    {
        public static bool IsEqual(this Color32 color2, Color32 color1)
        {
            return color1.r == color2.r &&
                   color1.b == color2.b &&
                   color1.g == color2.g;
        }

        public static Color Lighten(this Color color, float amount)
        {
            return new Color(
                Mathf.Clamp01(color.r + amount),
                Mathf.Clamp01(color.g + amount),
                Mathf.Clamp01(color.b + amount),
                color.a
            );
        }

        public static Color Desaturate(this Color color, float amount)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            s = Mathf.Clamp01(s - amount);
            return Color.HSVToRGB(h, s, v, false);
        }

        public static Color GetRandom(this List<Color> colors, Color excludedColor)
        {
            Color selectedColor = excludedColor;
            while (selectedColor == excludedColor)
            {
                selectedColor = colors[Random.Range(0, colors.Count)];
            }
            return selectedColor;
        }
		
		public static Color GetRandom(this List<Color> colors, List<Color> excludedColorList)
		{
			var selectedColor = colors[Random.Range(0, colors.Count)];
			while (excludedColorList.Contains(selectedColor))
			{
				selectedColor = colors[Random.Range(0, colors.Count)];
			}
			return selectedColor;
		}
		
		public static Color GetRandom(this List<Color> colors)
		{
			var selectedColor = colors[Random.Range(0, colors.Count)];
			return selectedColor;
		}
    }
}
