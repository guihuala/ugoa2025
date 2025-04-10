using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapGeneratorWindow : EditorWindow
{
    private GameObject prefabToSpawn; // 需要生成的预制件
    private float spacing = 1f; // 间距
    private List<Vector2Int> drawnPositions = new List<Vector2Int>();
    private Vector2Int lastDrawnPosition;
    private int brushSize = 1;
    private Vector2Int? rectangleStart = null;
    
    // 绘制模式枚举
    private enum DrawMode { Freeform, Rectangle, Erase }
    private DrawMode currentMode = DrawMode.Freeform;
    
    // 连续擦除标志
    private bool continuousErasing = false;
    private bool isDrawing = false;
    
    // 绘制区域大小
    private int gridWidth = 20;
    private int gridHeight = 20;
    private bool hasSetGridSize = false;
    
    // 翻转选项
    private bool flipHorizontally = false;
    private bool flipVertically = false;

    [MenuItem("Tools/Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<MapGeneratorWindow>("Map Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("地图绘制器", EditorStyles.boldLabel);

        // 如果还没设置网格大小，显示设置界面
        if (!hasSetGridSize)
        {
            DrawGridSizeSettings();
            return;
        }

        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("生成的预制件", prefabToSpawn, typeof(GameObject), false);
        spacing = EditorGUILayout.FloatField("间距", spacing);
        brushSize = EditorGUILayout.IntSlider("笔刷大小", brushSize, 1, 5);
        
        flipHorizontally = EditorGUILayout.Toggle("水平翻转生成", flipHorizontally);
        flipVertically = EditorGUILayout.Toggle("垂直翻转生成", flipVertically);

        // 绘制模式选择
        EditorGUILayout.LabelField("绘制模式:");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(currentMode == DrawMode.Freeform, "自由绘制", "Button")) currentMode = DrawMode.Freeform;
        if (GUILayout.Toggle(currentMode == DrawMode.Rectangle, "矩形绘制", "Button")) currentMode = DrawMode.Rectangle;
        if (GUILayout.Toggle(currentMode == DrawMode.Erase, "擦除模式", "Button")) currentMode = DrawMode.Erase;
        EditorGUILayout.EndHorizontal();

        continuousErasing = EditorGUILayout.Toggle("连续擦除", continuousErasing);

        EditorGUILayout.HelpBox($"当前网格大小: {gridWidth}x{gridHeight}\n操作说明:\n" +
                               "- 自由绘制: 点击或拖动绘制\n" +
                               "- 矩形绘制: 点击确定起点，拖动确定大小\n" +
                               "- 擦除模式: 点击擦除 (启用连续擦除可拖动擦除)", MessageType.Info);

        // 创建一个绘制区域
        Rect drawArea = GUILayoutUtility.GetRect(300, 300, GUILayout.ExpandWidth(true));
        GUI.Box(drawArea, "绘制区域");

        // 处理绘制区域的鼠标事件
        HandleDrawingEvents(drawArea);

        if (GUILayout.Button("生成地图"))
        {
            if (prefabToSpawn == null)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个预制件！", "确定");
                return;
            }
            if (drawnPositions.Count == 0)
            {
                EditorUtility.DisplayDialog("错误", "请先绘制一些地形！", "确定");
                return;
            }
            GenerateMap();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("清除绘制"))
        {
            drawnPositions.Clear();
            rectangleStart = null;
            Repaint();
        }
        if (GUILayout.Button("填充整个区域"))
        {
            FillEntireArea();
        }
        if (GUILayout.Button("重新设置网格大小"))
        {
            hasSetGridSize = false;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGridSizeSettings()
    {
        EditorGUILayout.HelpBox("请先设置绘制区域大小", MessageType.Info);
        
        gridWidth = EditorGUILayout.IntField("网格宽度", gridWidth);
        gridHeight = EditorGUILayout.IntField("网格高度", gridHeight);
        
        gridWidth = Mathf.Clamp(gridWidth, 5, 50);
        gridHeight = Mathf.Clamp(gridHeight, 5, 50);
        
        if (GUILayout.Button("确认并开始绘制"))
        {
            hasSetGridSize = true;
            drawnPositions.Clear();
        }
    }

    private void HandleDrawingEvents(Rect drawArea)
    {
        Event evt = Event.current;
        Vector2 mousePos = evt.mousePosition;

        // 检查鼠标是否在绘制区域内
        if (!drawArea.Contains(mousePos))
        {
            // 如果鼠标离开区域且正在绘制矩形，则完成矩形绘制
            if (rectangleStart != null && evt.type == EventType.MouseUp)
            {
                rectangleStart = null;
                evt.Use();
            }
            return;
        }

        // 将鼠标位置转换为网格坐标
        Vector2 localMousePos = mousePos - drawArea.position;
        Vector2Int gridPos = new Vector2Int(
            Mathf.FloorToInt(localMousePos.x / (drawArea.width / gridWidth)),
            Mathf.FloorToInt(localMousePos.y / (drawArea.height / gridHeight))
        );

        // 确保坐标在网格范围内
        gridPos.x = Mathf.Clamp(gridPos.x, 0, gridWidth - 1);
        gridPos.y = Mathf.Clamp(gridPos.y, 0, gridHeight - 1);

        switch (currentMode)
        {
            case DrawMode.Freeform:
                HandleFreeformDrawing(evt, gridPos);
                break;
            case DrawMode.Rectangle:
                HandleRectangleDrawing(evt, gridPos);
                break;
            case DrawMode.Erase:
                HandleErasing(evt, gridPos);
                break;
        }

        // 绘制网格和已绘制的点
        DrawGrid(drawArea);
        DrawDrawnPositions(drawArea);
        
        // 如果是矩形模式且正在绘制，显示预览
        if (rectangleStart != null && currentMode == DrawMode.Rectangle)
        {
            DrawRectanglePreview(drawArea, rectangleStart.Value, gridPos);
        }

        // 强制重绘以实现实时更新
        if (evt.type == EventType.MouseMove)
            Repaint();
    }

    private void HandleFreeformDrawing(Event evt, Vector2Int gridPos)
    {
        if (evt.type == EventType.MouseDown && evt.button == 0)
        {
            isDrawing = true;
            AddBrushStroke(gridPos, false);
            evt.Use();
        }
        else if (evt.type == EventType.MouseUp && evt.button == 0)
        {
            isDrawing = false;
            evt.Use();
        }
        else if (evt.type == EventType.MouseDrag && isDrawing && evt.button == 0)
        {
            AddBrushStroke(gridPos, false);
            evt.Use();
        }
    }

    private void HandleRectangleDrawing(Event evt, Vector2Int gridPos)
    {
        if (evt.type == EventType.MouseDown && evt.button == 0)
        {
            rectangleStart = gridPos;
            evt.Use();
        }
        else if (evt.type == EventType.MouseUp && evt.button == 0 && rectangleStart != null)
        {
            FillRectangle(rectangleStart.Value, gridPos, false);
            rectangleStart = null;
            evt.Use();
        }
        else if (evt.type == EventType.MouseDrag && rectangleStart != null && evt.button == 0)
        {
            // 拖动时只更新显示，不实际添加点，直到鼠标释放
            evt.Use();
        }
    }

    private void HandleErasing(Event evt, Vector2Int gridPos)
    {
        if (evt.type == EventType.MouseDown && (evt.button == 0 || evt.button == 1))
        {
            isDrawing = true;
            AddBrushStroke(gridPos, true);
            evt.Use();
        }
        else if (evt.type == EventType.MouseUp && (evt.button == 0 || evt.button == 1))
        {
            isDrawing = false;
            evt.Use();
        }
        else if (continuousErasing && evt.type == EventType.MouseDrag && isDrawing && (evt.button == 0 || evt.button == 1))
        {
            AddBrushStroke(gridPos, true);
            evt.Use();
        }
    }

    private void AddBrushStroke(Vector2Int center, bool erase)
    {
        // 防止重复处理同一位置
        if (center == lastDrawnPosition) return;
        lastDrawnPosition = center;

        for (int x = -brushSize + 1; x < brushSize; x++)
        {
            for (int y = -brushSize + 1; y < brushSize; y++)
            {
                Vector2Int pos = center + new Vector2Int(x, y);
                
                // 确保位置在网格范围内
                if (pos.x < 0 || pos.x >= gridWidth || pos.y < 0 || pos.y >= gridHeight)
                    continue;
                
                if (erase)
                {
                    drawnPositions.Remove(pos);
                }
                else if (!drawnPositions.Contains(pos))
                {
                    drawnPositions.Add(pos);
                }
            }
        }
        Repaint();
    }

    private void FillRectangle(Vector2Int start, Vector2Int end, bool erase)
    {
        // 确定矩形的边界
        int minX = Mathf.Min(start.x, end.x);
        int maxX = Mathf.Max(start.x, end.x);
        int minY = Mathf.Min(start.y, end.y);
        int maxY = Mathf.Max(start.y, end.y);

        // 确保边界在网格范围内
        minX = Mathf.Clamp(minX, 0, gridWidth - 1);
        maxX = Mathf.Clamp(maxX, 0, gridWidth - 1);
        minY = Mathf.Clamp(minY, 0, gridHeight - 1);
        maxY = Mathf.Clamp(maxY, 0, gridHeight - 1);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                
                if (erase)
                {
                    drawnPositions.Remove(pos);
                }
                else if (!drawnPositions.Contains(pos))
                {
                    drawnPositions.Add(pos);
                }
            }
        }
        Repaint();
    }

    private void FillEntireArea()
    {
        drawnPositions.Clear();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                drawnPositions.Add(new Vector2Int(x, y));
            }
        }
        Repaint();
    }

    private void DrawGrid(Rect area)
    {
        Handles.BeginGUI();
        Handles.color = Color.gray;

        // 计算每个格子的大小
        float cellWidth = area.width / gridWidth;
        float cellHeight = area.height / gridHeight;

        // 绘制垂直线
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(area.x + x * cellWidth, area.y, 0);
            Vector3 end = new Vector3(area.x + x * cellWidth, area.y + area.height, 0);
            Handles.DrawLine(start, end);
        }

        // 绘制水平线
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(area.x, area.y + y * cellHeight, 0);
            Vector3 end = new Vector3(area.x + area.width, area.y + y * cellHeight, 0);
            Handles.DrawLine(start, end);
        }

        Handles.EndGUI();
    }

    private void DrawDrawnPositions(Rect area)
    {
        Handles.BeginGUI();
        
        // 计算每个格子的大小
        float cellWidth = area.width / gridWidth;
        float cellHeight = area.height / gridHeight;

        foreach (Vector2Int pos in drawnPositions)
        {
            Vector3 center = new Vector3(
                area.x + pos.x * cellWidth + cellWidth / 2,
                area.y + pos.y * cellHeight + cellHeight / 2,
                0
            );
            
            Handles.color = Color.green;
            Handles.DrawSolidDisc(center, Vector3.forward, Mathf.Min(cellWidth, cellHeight) / 3);
        }

        Handles.EndGUI();
    }

    private void DrawRectanglePreview(Rect area, Vector2Int start, Vector2Int end)
    {
        Handles.BeginGUI();
        Handles.color = new Color(0, 1, 0, 0.2f);

        // 计算每个格子的大小
        float cellWidth = area.width / gridWidth;
        float cellHeight = area.height / gridHeight;

        // 计算矩形边界
        int minX = Mathf.Min(start.x, end.x);
        int maxX = Mathf.Max(start.x, end.x);
        int minY = Mathf.Min(start.y, end.y);
        int maxY = Mathf.Max(start.y, end.y);

        // 计算屏幕坐标
        float x = area.x + minX * cellWidth;
        float y = area.y + minY * cellHeight;
        float width = (maxX - minX + 1) * cellWidth;
        float height = (maxY - minY + 1) * cellHeight;

        // 绘制半透明矩形
        Handles.DrawSolidRectangleWithOutline(
            new Rect(x, y, width, height),
            new Color(0, 1, 0, 0.1f),
            new Color(0, 1, 0, 0.5f)
        );

        Handles.EndGUI();
    }

    private void GenerateMap()
    {
        GameObject parent = new GameObject("GeneratedMap");
        
        // 找到绘制的边界来确定中心点
        int minX = int.MaxValue, maxX = int.MinValue;
        int minZ = int.MaxValue, maxZ = int.MinValue;
        
        foreach (Vector2Int pos in drawnPositions)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minZ) minZ = pos.y;
            if (pos.y > maxZ) maxZ = pos.y;
        }
        
        Vector2 centerOffset = new Vector2(
            (minX + maxX) * 0.5f,
            (minZ + maxZ) * 0.5f
        );

        foreach (Vector2Int pos in drawnPositions)
        {
            // 如果需要水平翻转，则计算翻转后的x坐标
            float xPos = flipHorizontally ? 
                (gridWidth - 1 - pos.x - centerOffset.x) * spacing : 
                (pos.x - centerOffset.x) * spacing;
            
            // 垂直翻转计算
            float zPos = flipVertically ?
                (gridHeight - 1 - pos.y - centerOffset.y) * spacing :
                (pos.y - centerOffset.y) * spacing;
            
            Vector3 position = new Vector3(
                xPos,
                0,
                zPos  // 使用计算后的z位置
            );
            
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
            instance.transform.position = position;
            instance.transform.SetParent(parent.transform);
        }
        
        Debug.Log($"生成了一个包含 {drawnPositions.Count} 个地块的地图！{(flipHorizontally ? " (已水平翻转)" : "")}");
    }
}