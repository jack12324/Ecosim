using UnityEngine;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;

namespace Scenes.Scripts
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, int seed)
        {
            var noiseMap = new float[mapWidth, mapHeight];
            var random = new Random(seed);
            var octaveOffsets = new Vector2[octaves];
            
            for (var octave = 0; octave < octaves; octave++)
            {
                var xOffset = random.Next(-100000, 100000) + offset.x;
                var yOffset = random.Next(-100000, 100000) + offset.y;
                octaveOffsets[octave] = new Vector2(xOffset, yOffset);
            }

            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;
            

            if (scale <= 0)
            {
                scale = 0.00003f;
            }
            
            var noiseHeightRange = new NoiseHeightRange(float.MaxValue, float.MinValue);

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    var noiseHeight = 0f;
                    var amplitude = 1f;
                    var frequency = 1f;
                    
                    for (var octave = 0; octave < octaves; octave++)
                    {
                        var xSample = ((x - halfWidth) / scale + octaveOffsets[octave].x) * frequency ;
                        var ySample = ((y - halfWidth) / scale + octaveOffsets[octave].y) * frequency ;
                        
                        var perlinValue = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;
                        
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                    
                    noiseMap[x, y] = noiseHeight;

                    noiseHeightRange.UpdateNoiseMapHeightRange(noiseHeight);
                }
            }

            return NormalizeMap(noiseMap, noiseHeightRange);
        }

        private static float[,] NormalizeMap(float[,] noiseMap, NoiseHeightRange noiseHeightRange)
        {
            for (var yIndex = 0; yIndex < noiseMap.GetLength(1); yIndex++)
            {
                for (var xIndex = 0; xIndex < noiseMap.GetLength(0); xIndex++)
                {
                    noiseMap[xIndex, yIndex] =
                        Mathf.InverseLerp(noiseHeightRange.MinimumValue, noiseHeightRange.MaximumValue, noiseMap[xIndex, yIndex]);
                }
            }

            return noiseMap;
        }
    }

    internal class NoiseHeightRange
    {
        public float MinimumValue { get; set; }
        public float MaximumValue { get; set; }

        public NoiseHeightRange(float minimumValue, float maximumValue)
        {
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
        }

        public void UpdateNoiseMapHeightRange(float newNoiseHeight)
        {
            if (newNoiseHeight < MinimumValue)
            {
                MinimumValue = newNoiseHeight;
            }
            else if (newNoiseHeight > MaximumValue)
            {
                MaximumValue = newNoiseHeight;
            }
        }
    }
}