using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCell
{
    public int Index;
    
    public string CharacterName;
    public NPCData NPC;
    public int SelectedVariantIndex; // ѡ��� NPC �������
    
    public CellType CellType;
    public int JumpToIndex;
    public CellFlag CellFlag;

    [TextArea(3, 5)]
    public string Content;

    

    public Sprite CharacterSprite // ��ȡ�������
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
    Standard,//��׼��һ��Ի�
    Select,//�þ�Ի���һ�����Ҫѡ���ѡ��
}

public enum CellFlag
{
    Begin,
    End,
    None,
}
