using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public static class OpenPersistentDataPathUtility
{
    [MenuItem("Tools/打开存储数据路径", false, 10)]
    public static void OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath;
        UnityEngine.Debug.Log("Opening: " + path);

#if UNITY_EDITOR_WIN
            Process.Start("explorer.exe", path);
#elif UNITY_EDITOR_OSX
        Process.Start("open", path);
#elif UNITY_EDITOR_LINUX
            Process.Start("xdg-open", path);
#endif
    }
}