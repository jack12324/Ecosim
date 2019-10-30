using UnityEngine;

public class MapAttributes
{
    public int MapSideLength { get; set; }
    public float NoiseScale { get; set; }
    public int Octaves { get; set; }
    public float Persistence { get; set; }
    public float Lacunarity { get; set; }
    public int Seed { get; set; }
    public Offset Offset { get; set; }

    public MapAttributes(int mapSideLength, float noiseScale, int octaves, float persistence, float lacunarity, Vector2 offset, int seed)
    {
        MapSideLength = mapSideLength;
        NoiseScale = noiseScale;
        Octaves = octaves;
        Persistence = persistence;
        Lacunarity = lacunarity;
        Seed = seed;
        Offset = new Offset(offset.x, offset.y);
    }

    public override string ToString()
    {
        return $"mapSideLength: {MapSideLength}\n" +
               $"NoiseScale: {NoiseScale}\n" +
               $"Octaves: {Octaves}\n" +
               $"Persistence: {Persistence}\n" +
               $"Lacunarity: {Lacunarity}\n" +
               $"Offset: x:{Offset.x} y:{Offset.y}\n" +
               $"Seed: {Seed}\n";
    }

    public MapAttributes DeepCopy()
    {
        return new MapAttributes(MapSideLength,  NoiseScale, Octaves, Persistence, Lacunarity, new Vector2(Offset.x, Offset.y), Seed);
    }  
}
public struct Offset
{
    public float x;
    public float y;

    public Offset(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}