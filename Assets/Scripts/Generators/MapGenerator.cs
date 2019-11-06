using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Generators
{
    public class MapGenerator : MonoBehaviour
    {
        public enum DrawMode
        {
            NoiseMap = 0,
            ColorMap = 1,
            Mesh = 2
        };

        public DrawMode drawMode;

        public const int MapChunkSize = 241;
        [Range(0,6)]
        public int levelOfDetail;
        public float noiseScale;
        public bool autoUpdate;
        public int octaves;
        
        [UnityEngine.Range(0, 1)]
        public float persistence;
        
        public float lacunarity;
        public int seed;
        public Vector2 offset;

        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        public TerrainType[] regions;

        public void DrawSelectedMapType()
        {
            var mapData = GenerateMapData();
            
            var mapDisplay= GetComponent<MapDisplay>();
            
            if (drawMode == DrawMode.NoiseMap)
            {
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.NoiseMap));
            }
            else if (drawMode == DrawMode.ColorMap)
            {
                mapDisplay.DrawTexture(
                    TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapChunkSize, MapChunkSize));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                mapDisplay.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(mapData.NoiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),
                    TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapChunkSize, MapChunkSize));
            }
        }

        private MapData GenerateMapData()
        {
            
            var sysRand = new System.Random(seed);
            var attributes = new MapAttributes(MapChunkSize, noiseScale, octaves, persistence, lacunarity, offset,
                sysRand.Next());
            var noiseMapFlat = new NativeArray<float>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob);
            
            var colorMap = new NativeArray<Color>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob);
            var terrainRegions = new NativeArray<TerrainType>(this.regions.Length, Allocator.TempJob);
            terrainRegions.CopyFrom(regions.OrderBy(region => region.maxHeight).ToArray());

            var mapDataJob = new Noise.GenerateNoiseMapJob
            {
                sideLength = attributes.MapSideLength,
                lacunarity = attributes.Lacunarity,
                NoiseMap = noiseMapFlat,
                noiseScale = attributes.NoiseScale,
                Octaves = attributes.Octaves,
                offset = attributes.Offset,
                persistence = attributes.Persistence,
                Seed = (uint)attributes.Seed
            };
            var colorMapJob = new TextureGenerator.ColorMapFromTerrainsJob
            {
                NoiseMapFlat = noiseMapFlat,
                SortedRegions = terrainRegions,
                ColorMap = colorMap
            };

            var handle = colorMapJob.Schedule(mapDataJob.Schedule());
            handle.Complete();

            var mapData = new MapData(noiseMapFlat.ToArray(), colorMap.ToArray(), attributes.MapSideLength);
                
            noiseMapFlat.Dispose();
            colorMap.Dispose();
            terrainRegions.Dispose();

            return mapData;
        }

        private void OnValidate()
        {
            octaves = octaves < 1 ? 1 : octaves;
            lacunarity = lacunarity < 1 ? 1 : lacunarity;
            noiseScale = noiseScale < 0 ? 0 : noiseScale;
        }
        public struct MapData
        {
            public readonly float[,] NoiseMap;
            public readonly Color[] ColorMap;
            public MapData(IReadOnlyList<float> noiseMapFlat, Color[] colorMap, int width)
            {
                NoiseMap = new float[width, width];
                var index = 0;
                for (var row = 0; row < NoiseMap.GetLength(0); row++)
                {
                    for (var col = 0; col < NoiseMap.GetLength(1); col++)
                    {
                        NoiseMap[row, col] = noiseMapFlat[index];
                        ++index;
                    }
                }
                this.ColorMap = colorMap;
            }
        }
    }
}