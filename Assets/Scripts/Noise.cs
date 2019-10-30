using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public static class Noise
{
   
    
    [BurstCompile]
    public struct  GenerateNoiseMapJob: IJob
    {
        [WriteOnly]
        public NativeArray<float> NoiseMap;
        public uint Seed;
        public int Octaves;
        public Offset offset;
        public int sideLength;
        public float noiseScale;
        public float persistence;
        public float lacunarity;
        
        
        public void Execute()
        {
            var  rand = new Unity.Mathematics.Random(Seed);
            var xOffset = rand.NextInt(-100000, 100000) + offset.x;
            var yOffset = rand.NextInt(-100000, 100000) + offset.y;

            var halfLength = sideLength / 2;


            if (noiseScale <= 0)
            {
                noiseScale = 0.00003f;
            }

            float maxAmplitude = Octaves;
            if (Math.Abs(persistence - 1) > .001f)
            {
                //This Calculation is derived from the definition of a Geometric Sum
                maxAmplitude = (float)(1 - Math.Pow(persistence, Octaves)) / (1 - persistence);
            }

            var index = 0;
            for (var row= 0; row< sideLength; row++)
            {
                for (var col = 0; col < sideLength; col++)
                {
                    var noiseHeight = 0f;
                    var amplitude = 1f;
                    var frequency = 1f;
                        
                    for (var octave = 0; octave < Octaves; octave++)
                    {
                        var ySample = ((row- halfLength) / noiseScale + yOffset) * frequency ;
                        var xSample = ((col - halfLength) / noiseScale + xOffset) * frequency ;

                        var perlinValue = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;

                        if (perlinValue <= -2 || perlinValue >= 2)
                        {
                            //The PerlinNoise Function has an input range of -1e10 to 1e10, anything outside this range
                            //causes A very large number to come out, ruining the map
                            perlinValue = 0;
                        }
                        
                        noiseHeight += perlinValue * amplitude;
                            
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    NoiseMap[index] = Mathf.InverseLerp(-maxAmplitude, maxAmplitude, noiseHeight);
                    index++;
                }
            }
        }
       
    }
}

