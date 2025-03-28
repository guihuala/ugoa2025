using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Camera = UnityEngine.Camera;

public static class SAVE
{
    static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    #region PlayerPrefs存储

    public static void PlayerPrefsSave(string key, object data)
    {
        // 将对象数据序列化为JSON字符串保存
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public static string PlayerPrefsLoad(string key)
    {
        // 从PlayerPrefs读取字符串，默认值为null
        return PlayerPrefs.GetString(key, null);
    }

    public static void PlayerPrefsDelete(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    #endregion

    #region JSON存储

    public static void JsonSave(string fileName, object data)
    {
        try
        {
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);
            string directory = Path.GetDirectoryName(fullPath);

            // 确保目录存在
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 序列化数据
            string jsonData = JsonUtility.ToJson(data, prettyPrint: true);

            // 写入文件
            File.WriteAllText(fullPath, jsonData);
            Debug.Log($"存档成功: {fullPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"存档失败: {ex.GetType().Name} - {ex.Message}");
        }
    }


    public static T JsonLoad<T>(string fileName)
    {
        string path = GetPath(fileName);
        // 如果文件存在则读取
        if (File.Exists(path))
        {
            string json = File.ReadAllText(GetPath(fileName));
            var data = JsonUtility.FromJson<T>(json);
            return data;
        }

        // 对象类型返回null，值类型返回默认值
        return default;
    }

    public static void JsonDelete(string fileName)
    {
        File.Delete(GetPath(fileName));
    }

    #endregion

    #region 存档导入导出功能

    /// <summary>
    /// 导出存档到指定路径
    /// </summary>
    public static bool ExportSave(string fileName, string exportPath)
    {
        try
        {
            string sourcePath = GetPath(fileName);
            if (!File.Exists(sourcePath))
            {
                Debug.LogWarning($"导出失败: 存档文件 {fileName} 不存在");
                return false;
            }

            // 确保目标目录存在
            string directory = Path.GetDirectoryName(exportPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Copy(sourcePath, exportPath, overwrite: true);
            Debug.Log($"存档导出成功: {exportPath}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"存档导出失败: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 从外部文件导入存档
    /// </summary>
    public static bool ImportSave(string importPath, string fileName)
    {
        try
        {
            if (!File.Exists(importPath))
            {
                Debug.LogWarning($"导入失败: 源文件 {importPath} 不存在");
                return false;
            }

            string destPath = GetPath(fileName);

            // 确保目标目录存在
            string directory = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Copy(importPath, destPath, overwrite: true);
            Debug.Log($"存档导入成功: {destPath}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"存档导入失败: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取存档文件列表
    /// </summary>
    public static List<string> GetSaveFiles()
    {
        List<string> saveFiles = new List<string>();

        try
        {
            var files = Directory.GetFiles(Application.persistentDataPath, "*.save");
            saveFiles.AddRange(files);
        }
        catch (Exception ex)
        {
            Debug.LogError($"获取存档列表失败: {ex.Message}");
        }

        return saveFiles;
    }

    #endregion

    #region 清理功能

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Delete/Records List")]
    public static void DeleteRecord()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
        Debug.Log("已清空存档列表");
    }

    [UnityEditor.MenuItem("Delete/Player Data")]
    public static void DeletePlayerData()
    {
        ClearDirectory(Application.persistentDataPath);
        Debug.Log("已清空玩家数据");
    }

    static void ClearDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            FileInfo[] f = new DirectoryInfo(path).GetFiles("*");
            for (int i = 0; i < f.Length; i++)
            {
                Debug.Log($"删除 {f[i].Name}");
                File.Delete(f[i].FullName);
            }
        }
    }

    [UnityEditor.MenuItem("Delete/All")]
    public static void DeleteAll()
    {
        DeletePlayerData();
        DeleteRecord();
    }
#endif

    #endregion
}