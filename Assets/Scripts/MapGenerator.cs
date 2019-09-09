using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public bool autoUpdate;
    public int octaves;
        
    [Range(0, 1)]
    public float persistence;
        
    public float lacunarity;
    public int seed;
    public int offsetX;
    public int offsetY;


    public void GenerateMap()
    {
        var offset = new Vector2(offsetX, offsetY);
        var mapAttributes = new MapAttributes(mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, offset, seed);
        var noiseMap = Noise.GenerateNoiseMap(mapAttributes);

        var mapDisplay= GetComponent<MapDisplay>();
        mapDisplay.DrawNoiseMap(noiseMap);
    }

    private void OnValidate()
    {
        mapWidth = mapWidth < 1 ? 1 : mapWidth;
        mapHeight = mapHeight < 1 ? 1 : mapHeight;
        octaves = octaves < 1 ? 1 : octaves;
        lacunarity = lacunarity < 1 ? 1 : lacunarity;
        noiseScale = noiseScale < 0 ? 0 : noiseScale;
    }
}