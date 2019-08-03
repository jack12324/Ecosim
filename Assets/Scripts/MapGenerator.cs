using UnityEngine;

namespace Scenes.Scripts
{
    public class MapGenerator : MonoBehaviour
    {
        public int mapWidth;
        public int mapHeight;
        public float noiseScale;
        public bool autoUpdate;

        public void GenerateMap()
        {
            var noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);

            var mapDisplay= GetComponent<MapDisplay>();
            mapDisplay.DrawNoiseMap(noiseMap);
        }
    }
}