using UnityEngine;
using UnityEditor;

public class MapGeneratorWindow : EditorWindow
{
    private GameObject prefabToSpawn; // 需要生成的预制件
    private int width = 10; // 地图宽度
    private int height = 10; // 地图高度
    private float spacing = 1.5f; // 间距

    [MenuItem("Tools/Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<MapGeneratorWindow>("Map Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("地图生成器", EditorStyles.boldLabel);

        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("生成的预制件", prefabToSpawn, typeof(GameObject), false);

        width = EditorGUILayout.IntField("宽度", width);
        height = EditorGUILayout.IntField("高度", height);
        spacing = EditorGUILayout.FloatField("间距", spacing);

        if (GUILayout.Button("生成地图"))
        {
            if (prefabToSpawn == null)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个预制件！", "确定");
                return;
            }
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        GameObject parent = new GameObject("GeneratedMap");
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
                instance.transform.position = position;
                instance.transform.SetParent(parent.transform);
            }
        }
        Debug.Log($"生成了一个 {width} x {height} 的地图！");
    }
}