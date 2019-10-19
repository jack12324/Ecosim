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


        public void GenerateMap()
        {
            var mapAttributes = new MapAttributes(MapChunkSize, MapChunkSize, noiseScale, octaves, persistence, lacunarity, offset, seed);
            var noiseMap = Noise.GenerateNoiseMap(mapAttributes);

            var colorMap = TextureGenerator.ColorMapFromTerrains(noiseMap, regions);

            var mapDisplay= GetComponent<MapDisplay>();

            if (drawMode == DrawMode.NoiseMap)
            {
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            }
            else if (drawMode == DrawMode.ColorMap)
            {
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapAttributes.MapWidth, mapAttributes.MapHeight));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),
                    TextureGenerator.TextureFromColorMap(colorMap, mapAttributes.MapWidth, mapAttributes.MapHeight));
            }
        }

        private void OnValidate()
        {
            octaves = octaves < 1 ? 1 : octaves;
            lacunarity = lacunarity < 1 ? 1 : lacunarity;
            noiseScale = noiseScale < 0 ? 0 : noiseScale;
        }
    }
}