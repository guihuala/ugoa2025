using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordData : SingletonPersistent<RecordData>
{
    public const int recordNum = 5;             // 存档数量
    public const string NAME = "RecordData";     // 存档的键名

    public string[] recordName = new string[recordNum];    // 存档文件名（完整路径）
    public int lastID;                                     // 最新的存档编号（用于新建存档时自动生成）

    // 存档数据类
    class SaveData
    {
        public string[] recordName = new string[recordNum];
        public int lastID;
    }

    // 将当前数据转换为保存格式
    SaveData ForSave()
    {
        var savedata = new SaveData();

        for (int i = 0; i < recordNum; i++)
        {
            savedata.recordName[i] = recordName[i];
        }
        savedata.lastID = lastID;

        return savedata;
    }

    // 从保存数据加载到当前对象
    void ForLoad(SaveData savedata)
    {
        lastID = savedata.lastID;
        for (int i = 0; i < recordNum; i++)
        {
            recordName[i] = savedata.recordName[i];
        }
    }

    // 保存数据到 PlayerPrefs
    public void Save()
    {
        SAVE.PlayerPrefsSave(NAME, ForSave());
    }

    // 从 PlayerPrefs 加载数据
    public void Load()
    {
        // 如果存在存档数据
        if (PlayerPrefs.HasKey(NAME))
        {
            string json = SAVE.PlayerPrefsLoad(NAME);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            ForLoad(saveData);
        }
    }
}