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


        private MapData GenerateMapData()
        {
            var mapAttributes = new MapAttributes(MapChunkSize, MapChunkSize, noiseScale, octaves, persistence, lacunarity, offset, seed);
            var noiseMap = Noise.GenerateNoiseMap(mapAttributes);

            var colorMap = TextureGenerator.ColorMapFromTerrains(noiseMap, regions);

            return new MapData(noiseMap, colorMap);
            
        }

        public void DrawSelectedMapType()
        {
            var mapData = GenerateMapData();
            var mapDisplay= GetComponent<MapDisplay>();
            
            if (drawMode == DrawMode.NoiseMap)
            {
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.noiseMap));
            }
            else if (drawMode == DrawMode.ColorMap)
            {
                mapDisplay.DrawTexture(
                    TextureGenerator.TextureFromColorMap(mapData.colorMap, MapChunkSize, MapChunkSize));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                mapDisplay.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),
                    TextureGenerator.TextureFromColorMap(mapData.colorMap, MapChunkSize, MapChunkSize));
            }
        }

        private void OnValidate()
        {
            octaves = octaves < 1 ? 1 : octaves;
            lacunarity = lacunarity < 1 ? 1 : lacunarity;
            noiseScale = noiseScale < 0 ? 0 : noiseScale;
        }

        private struct MapData
        {
            public readonly float[,] noiseMap;
            public readonly Color[] colorMap;
            public MapData(float[,] noiseMap, Color[] colorMap)
            {
                this.noiseMap = noiseMap;
                this.colorMap = colorMap;
            }
        }
    }
}