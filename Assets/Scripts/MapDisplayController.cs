using UnityEngine;

namespace Scenes.Scripts
{
    public interface IMapDisplayController
    {
        Texture2D GenerateTexture(float[,] noiseMap);
    }

    public class MapDisplayController : IMapDisplayController
    {
        private int _mapWidth;
        private int _mapHeight;
        private float[,] _noiseMap;

        private Color[] GenerateColorMap()
        {
            var colorMap = new Color[_mapWidth * _mapHeight];

            for (var y = 0; y < _mapHeight; y++)
            {
                for (var x = 0; x < _mapWidth; x++)
                {
                    colorMap[y * _mapWidth + x] = Color.Lerp(Color.black, Color.white, _noiseMap[x, y]);
                }
            }

            return colorMap;
        }

        public Texture2D GenerateTexture(float[,] noiseMap)
        {
            _noiseMap = noiseMap;
            _mapWidth = noiseMap.GetLength(0);
            _mapHeight = _noiseMap.GetLength(1);
        
            var colorMap = GenerateColorMap();
            
            var texture2D = new Texture2D(_mapWidth, _mapHeight);

            texture2D.SetPixels(colorMap);
            texture2D.Apply();
            return texture2D;
        }
    }
}