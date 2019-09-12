using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        var texture2D = new Texture2D(width, height);

        texture2D.SetPixels(colorMap);
        texture2D.Apply();
        return texture2D;
    }
    
    private static Color[] GenerateColorMapFromNoiseMap(float[,] noiseMap, int mapWidth, int mapHeight)
    {
        
        var colorMap = new Color[mapWidth * mapHeight];

        for (var y = 0; y < mapHeight; y++)
        {
            for (var x = 0; x < mapWidth; x++)
            {
                colorMap[y * mapWidth + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        return colorMap;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        var mapWidth = heightMap.GetLength(0);
        var mapHeight = heightMap.GetLength(1);
        var colorMap = GenerateColorMapFromNoiseMap(heightMap, mapWidth, mapHeight);

        return TextureFromColorMap(colorMap, mapWidth, mapHeight);
    }
}