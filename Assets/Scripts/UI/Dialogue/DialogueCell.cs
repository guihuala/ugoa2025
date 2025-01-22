using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCell
{
    public int Index;
    
    public string CharacterName;
    public NPCData NPC;
    public int SelectedVariantIndex; // 选择的 NPC 差分索引
    
    public CellType CellType;
    public int JumpToIndex;
    public CellFlag CellFlag;

    [TextArea(3, 5)]
    public string Content;

    

    public Sprite CharacterSprite // 获取差分立绘
    {
        get
        {
            if (NPC != null && NPC.Variants != null && SelectedVariantIndex >= 0 && SelectedVariantIndex < NPC.Variants.Count)
            {
                return NPC.Variants[SelectedVariantIndex];
            }
            return null;
        }
    }
}


public enum CellType
{
    Standard,//标准的一句对话
    Select,//该句对话是一句玩家要选择的选项
}

public enum CellFlag
{
    Begin,
    End,
    None,
}
