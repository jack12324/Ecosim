using System;
using UnityEngine;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;

namespace Scenes.Scripts
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(MapAttributes attributes)
        {
            var noiseMap = new float[attributes.MapWidth, attributes.MapHeight];
            var random = new Random(attributes.Seed);
            var octaveOffsets = new Vector2[attributes.Octaves];
            
            for (var octave = 0; octave < attributes.Octaves; octave++)
            {
                var xOffset = random.Next(-100000, 100000) + attributes.Offset.x;
                var yOffset = random.Next(-100000, 100000) + attributes.Offset.y;
                octaveOffsets[octave] = new Vector2(xOffset, yOffset);
            }

            var halfWidth = attributes.MapWidth / 2;
            var halfHeight = attributes.MapHeight / 2;
            

            if (attributes.NoiseScale <= 0)
            {
                attributes.NoiseScale = 0.00003f;
            }
            
            
            float maxAmplitude = attributes.Octaves;
            if (!Mathf.Approximately(1, attributes.Persistence))
            {
                //This Calculation is derived from the definition of a Geometric Sum
                maxAmplitude = (float)(1 - Math.Pow(attributes.Persistence, attributes.Octaves)) / (1 - attributes.Persistence);
            }

            for (var y = 0; y < attributes.MapHeight; y++)
            {
                for (var x = 0; x < attributes.MapWidth; x++)
                {
                    var noiseHeight = 0f;
                    var amplitude = 1f;
                    var frequency = 1f;
                    
                    for (var octave = 0; octave < attributes.Octaves; octave++)
                    {
                        var xSample = ((x - halfWidth) / attributes.NoiseScale + octaveOffsets[octave].x) * frequency ;
                        var ySample = ((y - halfHeight) / attributes.NoiseScale + octaveOffsets[octave].y) * frequency ;
                        
                        var perlinValue = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;
                        
                        amplitude *= attributes.Persistence;
                        frequency *= attributes.Lacunarity;
                    }
                    
                    noiseMap[x, y] = Mathf.InverseLerp(-maxAmplitude, maxAmplitude, noiseHeight);
                }
            }
            
            return noiseMap;
        }
    }  
}