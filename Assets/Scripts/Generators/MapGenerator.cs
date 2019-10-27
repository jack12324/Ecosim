using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

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


        private struct GenerateMapDataJob : IJob
        {
            public MapAttributes MapAttributes;
            public NativeArray<float> NoiseMap;
            public NativeArray<Color> ColorMap;
            public NativeArray<TerrainType> Regions;

            public void Execute()
            {
                var noiseMapArray = Noise.GenerateNoiseMapFlat(MapAttributes);
                var colorMapArray = TextureGenerator.ColorMapFromTerrains(noiseMapArray, Regions);

                for (var i = 0; i < NoiseMap.Length; i++)
                {
                    NoiseMap[i] = noiseMapArray[i];
                    ColorMap[i] = colorMapArray[i];
                }
            }
        }

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
            var attributes = new MapAttributes(MapChunkSize, MapChunkSize, noiseScale, octaves, persistence, lacunarity, offset,
                seed);
            var terrainRegions = new NativeArray<TerrainType>(regions.Length, Allocator.TempJob);
            var noiseMapFlat = new NativeArray<float>(attributes.MapHeight * attributes.MapWidth, Allocator.TempJob);
            var colorMap = new NativeArray<Color>(attributes.MapHeight * attributes.MapWidth, Allocator.TempJob);
            for (var i = 0; i < terrainRegions.Length; i++)
                terrainRegions[i] = regions[i];

            var job = new GenerateMapDataJob
            {
                MapAttributes = attributes, Regions = terrainRegions, NoiseMap = noiseMapFlat, ColorMap = colorMap
            };

            JobHandle handle = job.Schedule();
            handle.Complete();

            var mapData = new MapData(noiseMapFlat.ToArray(), colorMap.ToArray(), MapChunkSize);
            
            terrainRegions.Dispose();
            noiseMapFlat.Dispose();
            colorMap.Dispose();
            
            return mapData;
        }

        private void OnValidate()
        {
            octaves = octaves < 1 ? 1 : octaves;
            lacunarity = lacunarity < 1 ? 1 : lacunarity;
            noiseScale = noiseScale < 0 ? 0 : noiseScale;
        }
        private struct MapData
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