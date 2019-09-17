using System.Linq;
using UnityEngine;

public partial class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap = 0,
        ColorMap = 1
    };

    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public bool autoUpdate;
    public int octaves;
        
    [UnityEngine.Range(0, 1)]
    public float persistence;
        
    public float lacunarity;
    public int seed;
    public float offsetX;
    public float offsetY;

    public TerrainType[] regions;


    public void GenerateMap()
    {
        var offset = new Vector2(offsetX, offsetY);
        var mapAttributes = new MapAttributes(mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, offset, seed);
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