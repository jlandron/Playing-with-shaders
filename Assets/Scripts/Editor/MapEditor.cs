using UnityEditor;
using UnityEngine;

namespace Shooter.Map
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MapGenerator map = target as MapGenerator;
            if (GUILayout.Button("Generate Map"))
            {
                map.GenerateMap();
            }
        }
    }
}
