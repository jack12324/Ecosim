using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generators
{
    public static class TextureGenerator
    {
        public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
        {
            var texture2D = new Texture2D(width, height);

            texture2D.SetPixels(colorMap);
            texture2D.filterMode = FilterMode.Point;
            texture2D.wrapMode = TextureWrapMode.Clamp;
            texture2D.Apply();
            return texture2D;
        }
    
        private static Color[] GenerateColorMapFromNoiseMap(float[,] noiseMap, int mapWidth, int mapHeight)
        {
        
            var colorMap = new Color[mapWidth * mapHeight];

            for (var row = 0; row < mapHeight; row++)
            {
                for (var col = 0; col < mapWidth; col++)
                {
                    colorMap[row * mapWidth + col] = Color.Lerp(Color.black, Color.white, noiseMap[col, row]);
                }
            }

            return colorMap;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            var mapWidth = heightMap.GetLength(0);
            var mapHeight = heightMap.GetLength(1);
            var colorMap = GenerateColorMapFromNoiseMap(heightMap, mapWidth, mapHeight);

            return TextureFromColorMap(colorMap, mapWidth, mapHeight);
        }

        public static Color[] ColorMapFromTerrains(float[] noiseMapFlat, IEnumerable<TerrainType> regions)
        {
            var sortedRegions = regions.OrderBy(region => region.maxHeight).ToList();

            var colorMap = new Color[noiseMapFlat.Length];
            for (var i = 0; i < noiseMapFlat.Length; i++)
            {
                    colorMap[i] = FindTerrainColor(noiseMapFlat[i], sortedRegions);
            }

            return colorMap;
        }

        private static Color FindTerrainColor(float noiseHeight, IEnumerable<TerrainType> sortedRegions)
        {

            foreach (var region in sortedRegions)
            {
                if (noiseHeight <= region.maxHeight)
                {
                    return region.color;
                }
            }
            return Color.white;
        }
    }
}