using System;
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

            var halfWidth = mapWidth / 2;
            var halfHeight = mapHeight / 2;
            

            if (scale <= 0)
            {
                scale = 0.00003f;
            }
            
            
            float maxAmplitude = octaves;
            if (!Mathf.Approximately(1, persistence))
            {
                //This Calculation is derived from the definition of a Geometric Sum
                maxAmplitude = (float)(1 - Math.Pow(persistence, octaves)) / (1 - persistence);
            }

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
                        var ySample = ((y - halfHeight) / scale + octaveOffsets[octave].y) * frequency ;
                        
                        var perlinValue = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;
                        
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                    
                    noiseMap[x, y] = Mathf.InverseLerp(-maxAmplitude, maxAmplitude, noiseHeight);
                }
            }
            
            return noiseMap;
        }
    }  
}