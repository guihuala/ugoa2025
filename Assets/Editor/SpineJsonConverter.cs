using UnityEditor;
using UnityEngine;
using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;

public class SpineJsonConverter : EditorWindow
{
    private string selectedFilePath = "";
    private string outputFilePath = "";

    [MenuItem("Tools/Spine 3.8 → 4.1 JSON Converter")]
    public static void ShowWindow()
    {
        GetWindow<SpineJsonConverter>("Spine JSON Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Spine 3.8 → 4.1 JSON 转换工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("选择 Spine 3.8 JSON 文件", GUILayout.Height(30)))
        {
            selectedFilePath = EditorUtility.OpenFilePanel("选择 Spine 3.8 JSON 文件", Application.dataPath, "json");
        }

        GUILayout.Space(5);
        EditorGUILayout.LabelField("已选择文件:", selectedFilePath, EditorStyles.wordWrappedLabel);

        if (!string.IsNullOrEmpty(selectedFilePath))
        {
            if (GUILayout.Button("转换为 4.1 版本", GUILayout.Height(30)))
            {
                ConvertSpineJson(selectedFilePath);
            }
        }

        if (!string.IsNullOrEmpty(outputFilePath))
        {
            GUILayout.Space(10);
            if (GUILayout.Button("打开转换后文件夹", GUILayout.Height(30)))
            {
                EditorUtility.RevealInFinder(outputFilePath);
            }
        }
    }

    private void ConvertSpineJson(string inputPath)
    {
        try
        {
            string jsonText = File.ReadAllText(inputPath);
            JObject data = JObject.Parse(jsonText);

            // 更新 Spine 版本
            if (data["skeleton"] != null && data["skeleton"]["spine"] != null)
            {
                data["skeleton"]["spine"] = "4.1";
            }

            // 转换 skins 结构（从 dict → list）
            if (data["skins"] is JObject oldSkins)
            {
                JArray newSkins = new JArray();
                foreach (var skin in oldSkins)
                {
                    newSkins.Add(new JObject
                    {
                        { "name", skin.Key },
                        { "attachments", skin.Value["attachments"] }
                    });
                }
                data["skins"] = newSkins;
            }

            // 生成输出文件路径
            string directory = Path.GetDirectoryName(inputPath);
            string outputFilename = Path.GetFileNameWithoutExtension(inputPath) + "_Spine4.1.json";
            outputFilePath = Path.Combine(directory, outputFilename);

            // 写入新的 JSON 文件
            File.WriteAllText(outputFilePath, data.ToString());

            EditorUtility.DisplayDialog("转换完成", $"文件已保存:\n{outputFilePath}", "OK");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("转换失败: " + ex.Message);
            EditorUtility.DisplayDialog("转换失败", "请检查 JSON 文件格式是否正确。\n错误: " + ex.Message, "OK");
        }
    }
}
