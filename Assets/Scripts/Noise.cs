using System;
using UnityEngine;
using Random = System.Random;

public static class Noise
{
    public static float[,] GenerateNoiseMap(MapAttributes attributes)
    {
        var noiseMap = new float[attributes.MapHeight, attributes.MapWidth];
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

        for (var row= 0; row< attributes.MapHeight; row++)
        {
            for (var col = 0; col < attributes.MapWidth; col++)
            {
                var noiseHeight = 0f;
                var amplitude = 1f;
                var frequency = 1f;
                    
                for (var octave = 0; octave < attributes.Octaves; octave++)
                {
                    var ySample = ((row- halfHeight) / attributes.NoiseScale + octaveOffsets[octave].y) * frequency ;
                    var xSample = ((col - halfWidth) / attributes.NoiseScale + octaveOffsets[octave].x) * frequency ;

                    var perlinValue = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;

                    if (perlinValue <= -2 || perlinValue >= 2)
                    {
                        //The PerlinNoise Function has an input range of -1e10 to 1e10, anything outside this range
                        //causes A very large number to come out, ruining the map
                        perlinValue = 0;
                    }
                    
                    noiseHeight += perlinValue * amplitude;
                        
                    amplitude *= attributes.Persistence;
                    frequency *= attributes.Lacunarity;
                }
                    
                noiseMap[row, col] = Mathf.InverseLerp(-maxAmplitude, maxAmplitude, noiseHeight);
            }
        }
            
        return noiseMap;
    }
}