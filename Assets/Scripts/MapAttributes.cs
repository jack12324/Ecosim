using UnityEngine;

namespace Scenes.Scripts
{
    public class MapAttributes
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public float NoiseScale { get; set; }
        public int Octaves { get; set; }
        public float Persistence { get; set; }
        public float Lacunarity { get; set; }
        public int Seed { get; set; }
        public Vector2 Offset { get; set; }

        public MapAttributes(int mapWidth, int mapHeight, float noiseScale, int octaves, float persistence, float lacunarity, Vector2 offset, int seed)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            NoiseScale = noiseScale;
            Octaves = octaves;
            Persistence = persistence;
            Lacunarity = lacunarity;
            Seed = seed;
            Offset = offset;
        }
    }
}