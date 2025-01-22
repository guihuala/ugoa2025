using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DialogueDataListSO", menuName = "CustomizedSO/DialogueDataListSO")]
public class DialogueBlock : ScriptableObject
{
    public List<DialogueCell> Cells;
}
