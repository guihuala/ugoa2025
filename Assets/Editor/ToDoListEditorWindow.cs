using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[Serializable]
public class ToDoItem
{
    public string taskName;
    public bool isCompleted;
}

public class ToDoListEditorWindow : EditorWindow
{
    private List<ToDoItem> toDoItems = new List<ToDoItem>(); // 存储任务列表
    private string newTask = ""; // 输入的新任务名称

    // 打开 To-Do List 编辑器窗口
    [MenuItem("Window/To-Do List")]
    public static void ShowWindow()
    {
        GetWindow<ToDoListEditorWindow>("To-Do List");
    }

    private void OnGUI()
    {
        GUILayout.Label("To-Do List", EditorStyles.boldLabel);

        // 输入框，用于输入新的任务
        newTask = EditorGUILayout.TextField("新任务", newTask);

        // 按钮，用于添加新任务
        if (GUILayout.Button("添加任务"))
        {
            if (!string.IsNullOrEmpty(newTask))
            {
                toDoItems.Add(new ToDoItem() { taskName = newTask, isCompleted = false });
                newTask = ""; // 清空输入框
            }
        }

        // 显示任务列表
        EditorGUILayout.Space();
        for (int i = 0; i < toDoItems.Count; i++)
        {
            ToDoItem task = toDoItems[i];

            // 显示任务的复选框和任务名称
            EditorGUILayout.BeginHorizontal();
            task.isCompleted = EditorGUILayout.Toggle(task.isCompleted, GUILayout.Width(20));
            task.taskName = EditorGUILayout.TextField(task.taskName);

            // 删除任务按钮
            if (GUILayout.Button("删除", GUILayout.Width(70)))
            {
                toDoItems.RemoveAt(i);
                return; // 删除后退出循环以避免索引错误
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
