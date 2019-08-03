using Scenes.Scripts;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var mapGen = (MapGenerator) target;

            if (DrawDefaultInspector())
            {
                if (mapGen.autoUpdate)
                {
                    mapGen.GenerateMap();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                mapGen.GenerateMap();
            }
        }
    }
}