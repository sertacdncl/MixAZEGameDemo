using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    public class LevelImportConfig : ScriptableObject
    {
#if UNITY_EDITOR
		[Reorderable] public GameObjectList Levels;

		public Color ColorChangeCellColor;
        public Color PlayerCellColor;

        [Serializable]
        public class GameObjectList : ReorderableArray<Object>
        {
        }

        public void GenerateAndSerialize()
        {
            string targetDirectory = $"{Application.dataPath}/_Game/Scripts/LevelImport/Resources/";

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            GenerateLevels();

            AssetDatabase.Refresh();
        }

		private void GenerateLevels()
        {
			for (int i = 0; i < Levels.Length; i++)
            {
                Debug.Log("Building Level: " + (i + 1));
                var assetObject = Levels[i];
				var level = GetLevel(assetObject as Sprite);
                var levelString = JsonUtility.ToJson(level);
                File.WriteAllText(
                    $"{Application.dataPath}/_Game/Scripts/LevelImport/Resources/{LevelImportTags.GeneratedLevel}{i}.txt",
                    levelString);
            }

            var levelInfoString = JsonUtility.ToJson(new LevelInfo(Levels.Length));
            File.WriteAllText(
                $"{Application.dataPath}/_Game/Scripts/LevelImport/Resources/{LevelImportTags.LevelInfo}.txt",
                levelInfoString);
        }

        public LevelDataContainer GetLevel(Sprite levelSprite)
        {
            var rect = levelSprite.rect;
            var movableCellPosList = new List<Vector2Int>();
            var playerCellPos = Vector2Int.zero;
			var colorChangeCellPos = Vector2Int.zero;
			var colorChangeExists = false;

            var minX = 99;
            var minY = 99;

            for (var col = 0; col < rect.width; col++)
            {
                for (var row = 0; row < rect.height; row++)
                {
                    var pixelColor = (Color32)levelSprite.texture.GetPixel(col + (int)rect.xMin, row + (int)rect.yMin);
                    var pixelPosition = new Vector2Int(col, row);

                    if(pixelColor.IsEqual(Color.black))
                        continue;

                    if(pixelColor.IsEqual(PlayerCellColor))
                        playerCellPos = pixelPosition;

					if (pixelColor.IsEqual(ColorChangeCellColor))
					{
						colorChangeCellPos = pixelPosition;
						colorChangeExists = true;
					}

                    movableCellPosList.Add(pixelPosition);

                    if (pixelPosition.x < minX)
                        minX = pixelPosition.x;

                    if (pixelPosition.y < minY)
                        minY = pixelPosition.y;
                }
            }

            for (var index = 0; index < movableCellPosList.Count; index++)
            {
                var cellPos = movableCellPosList[index];
                movableCellPosList[index] = new Vector2Int(cellPos.x - minX, cellPos.y - minY);
            }
            playerCellPos = new Vector2Int(playerCellPos.x - minX, playerCellPos.y - minY);
			if (colorChangeExists)
				colorChangeCellPos = new Vector2Int(colorChangeCellPos.x - minX, colorChangeCellPos.y - minY);
			else
				colorChangeCellPos = new Vector2Int(-99, -99);

            return new LevelDataContainer
            {
                MovableCellPosList = movableCellPosList,
                PlayerCellPos = playerCellPos,
				ColorChangeCellPos = colorChangeCellPos
            };
        }
#endif
    }

    [Serializable]
    public struct LevelInfo
    {
        public int LevelCount;
        public LevelInfo(int levelCount) => LevelCount = levelCount;
    }

    [Serializable]
    public struct LevelDataContainer
    {
        public List<Vector2Int> MovableCellPosList;
        public Vector2Int PlayerCellPos;
        public Vector2Int ColorChangeCellPos;
    }
}


