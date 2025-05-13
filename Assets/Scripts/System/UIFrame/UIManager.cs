using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonPersistent<UIManager>
{
    // 面板关闭事件
    public event Action<string> OnPanelClosed;

    // <面板名称, 面板预制体路径>
    private Dictionary<string, string> _panelPathDict;

    // 缓存的面板预制体 <面板名称, 面板预制体>
    private Dictionary<string, GameObject> _uiPrefabDict;

    // 当前已打开的面板实例 <面板名称, 面板实例>
    private Dictionary<string, BasePanel> _panelDict;

    // UI 面板的根节点
    private Transform _uiRoot;
    private Transform _persistentUIRoot; // 持久化UI根节点
    private GameObject _mainCanvas;
    public UIDatas uiDatas;
    
    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                _uiRoot = GameObject.Find("Canvas").transform;
                if (_uiRoot == null)
                {
                    Debug.LogError("场景中未找到名为 'Canvas' 的UI根节点");
                }
            }

            return _uiRoot;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        InitDicts();
    }
    
    public Transform PersistentUIRoot
    {
        get
        {
            if (_persistentUIRoot == null)
            {
                GameObject existingPersistentCanvas = GameObject.Find("PersistentCanvas");
                if (existingPersistentCanvas != null && existingPersistentCanvas.transform.parent == transform)
                {
                    _persistentUIRoot = existingPersistentCanvas.transform;
                    return _persistentUIRoot;
                }

                GameObject persistentRoot = new GameObject("PersistentCanvas");
                persistentRoot.transform.SetParent(transform);
                persistentRoot.transform.localPosition = Vector3.zero;
                persistentRoot.transform.localRotation = Quaternion.identity;
                persistentRoot.transform.localScale = Vector3.one;
            
                Canvas canvas = persistentRoot.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
                CanvasScaler scaler = persistentRoot.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            
                persistentRoot.AddComponent<GraphicRaycaster>();
                canvas.sortingOrder = 100;
            
                DontDestroyOnLoad(persistentRoot);
                _persistentUIRoot = persistentRoot.transform;
            }
            return _persistentUIRoot;
        }
    }


    
    private void InitDicts()
    {
        _panelPathDict = new Dictionary<string, string>();
        foreach (var data in uiDatas.uiDataList)
        {
            _panelPathDict.Add(data.uiName, data.uiPath);
        }

        _uiPrefabDict = new Dictionary<string, GameObject>();
        _panelDict = new Dictionary<string, BasePanel>();
    }

    /// <summary>
    /// 打开UI面板，自动清理无效面板引用
    /// </summary>
    public BasePanel OpenPanel(string name, bool asPersistent = false)
    {
        // 检查面板是否已经打开但实例已被销毁
        if (_panelDict.TryGetValue(name, out BasePanel panel))
        {
            if (panel == null || panel.gameObject == null)
            {
                _panelDict.Remove(name);
            }
            else
            {
                Debug.LogWarning($"面板 {name} 已经打开");
                return panel;
            }
        }

        if (!_panelPathDict.TryGetValue(name, out string path))
        {
            Debug.LogWarning($"面板 {name} 的路径不存在");
            return null;
        }

        // 从缓存中获取面板预制体
        if (!_uiPrefabDict.TryGetValue(name, out GameObject panelPrefab))
        {
            panelPrefab = Resources.Load<GameObject>(path);
            if (panelPrefab == null)
            {
                Debug.LogError($"面板 {name} 的预制体未找到：{path}");
                return null;
            }
            _uiPrefabDict.Add(name, panelPrefab);
        }

        // 选择正确的父节点并检查是否存在
        Transform parent = asPersistent ? PersistentUIRoot : UIRoot;
        if (parent == null)
        {
            Debug.LogError("无法找到有效的UI根节点");
            return null;
        }

        // 检查父节点下是否已存在同名面板（可能由其他方式创建）
        Transform existingPanel = parent.Find(name);
        if (existingPanel != null)
        {
            panel = existingPanel.GetComponent<BasePanel>();
            if (panel != null)
            {
                _panelDict[name] = panel;
                panel.OpenPanel(name);
                return panel;
            }
            else
            {
                Destroy(existingPanel.gameObject);
            }
        }

        // 实例化新面板
        GameObject panelObj = Instantiate(panelPrefab, parent, false);
        panel = panelObj.GetComponent<BasePanel>();

        if (panel == null)
        {
            Debug.LogError($"面板 {name} 的脚本未挂载或未继承 BasePanel");
            Destroy(panelObj);
            return null;
        }

        panelObj.name = name; // 确保对象名称一致
        panel.OpenPanel(name);
        _panelDict.Add(name, panel);

        if (asPersistent)
        {
            DontDestroyOnLoad(panelObj);
        }

        return panel;
    }

    /// <summary>
    /// 关闭UI面板，自动清理无效引用
    /// </summary>
    public bool ClosePanel(string name, bool destroyPersistent = false)
    {
        if (!_panelDict.TryGetValue(name, out BasePanel panel))
        {
            Debug.LogWarning($"面板 {name} 当前未打开，无法关闭");
            return false;
        }

        // 检查面板实例是否已被销毁
        if (panel == null || panel.gameObject == null)
        {
            _panelDict.Remove(name);
            return false;
        }

        _panelDict.Remove(name);
        panel.ClosePanel();

        // 如果是持久化面板且要求销毁
        if (destroyPersistent && panel.transform.IsChildOf(PersistentUIRoot))
        {
            Destroy(panel.gameObject);
        }

        OnPanelClosed?.Invoke(name);
        return true;
    }

    /// <summary>
    /// 移除面板引用但不销毁实例
    /// </summary>
    /// <param name="name">面板名称</param>
    /// <returns>是否移除成功</returns>
    public bool RemovePanel(string name)
    {
        if (!_panelDict.TryGetValue(name, out BasePanel panel))
        {
            Debug.LogWarning($"面板 {name} 不在已打开的字典中，无法移除");
            return false;
        }

        _panelDict.Remove(name);
        return true;
    }
    
    /// <summary>
    /// 检查并清理所有无效的面板引用
    /// </summary>
    public void CleanInvalidPanelReferences()
    {
        List<string> invalidPanels = new List<string>();

        foreach (var kvp in _panelDict)
        {
            if (kvp.Value == null || kvp.Value.gameObject == null)
            {
                invalidPanels.Add(kvp.Key);
            }
        }

        foreach (string panelName in invalidPanels)
        {
            _panelDict.Remove(panelName);
            Debug.Log($"已清理无效面板引用: {panelName}");
        }
    }

    /// <summary>
    /// 检查指定面板是否存在于场景中
    /// </summary>
    public bool IsPanelActiveInScene(string name)
    {
        if (_panelDict.TryGetValue(name, out BasePanel panel))
        {
            return panel != null && panel.gameObject != null;
        }
        return false;
    }

    /// <summary>
    /// 关闭所有面板，包括持久化面板
    /// </summary>
    /// <param name="destroyPersistent">是否销毁持久化面板</param>
    public void CloseAllPanels(bool destroyPersistent = false)
    {
        List<string> panelsToClose = new List<string>(_panelDict.Keys);
        
        foreach (string panelName in panelsToClose)
        {
            bool isPersistent = _panelDict[panelName].transform.IsChildOf(PersistentUIRoot);
            ClosePanel(panelName, destroyPersistent && isPersistent);
        }
    }
}