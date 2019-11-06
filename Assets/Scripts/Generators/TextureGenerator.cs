using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
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

        [BurstCompile]
        public struct ColorMapFromTerrainsJob :IJob
        {
            [ReadOnly]
            public NativeArray<TerrainType> SortedRegions;
            [ReadOnly]
            public NativeArray<float> NoiseMapFlat;
            [WriteOnly]
            public NativeArray<Color> ColorMap;
            public void Execute()
            {
                for (var i = 0; i < NoiseMapFlat.Length; i++)
                {
                    ColorMap[i] = FindTerrainColor(NoiseMapFlat[i], SortedRegions);
                }
            }
        }

        private static Color FindTerrainColor(float noiseHeight, NativeArray<TerrainType> sortedRegions)
        {
            for (var index = 0; index < sortedRegions.Length; index++)
            {
                var region = sortedRegions[index];
                if (noiseHeight <= region.maxHeight)
                {
                    return region.color;
                }
            }

            return Color.white;
        }
    }
}