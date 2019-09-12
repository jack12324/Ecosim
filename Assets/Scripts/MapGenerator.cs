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
    public int offsetX;
    public int offsetY;

    public TerrainType[] regions;


    public void GenerateMap()
    {
        var offset = new Vector2(offsetX, offsetY);
        var mapAttributes = new MapAttributes(mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, offset, seed);
        var noiseMap = Noise.GenerateNoiseMap(mapAttributes);
        
        var colorMap = new Color[mapAttributes.MapWidth * mapAttributes.MapHeight];
        for (var y = 0; y < mapAttributes.MapHeight; y++)
        {
            for (var x = 0; x < mapAttributes.MapWidth; x++)
            {
                colorMap[y * mapAttributes.MapWidth + x] = FindTerrainColor(noiseMap[x, y]);
            }
        }

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

    private  Color FindTerrainColor(float noiseHeight)
    {
        var sortedRegions = regions.OrderBy(region => region.maxHeight).ToList();

        foreach (var region in sortedRegions)
        {
            if (noiseHeight <= region.maxHeight)
            {
                return region.color;
            }
        }
        return Color.white;
    }


}