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

    public Transform PersistentUIRoot
    {
        get
        {
            if (_persistentUIRoot == null)
            {
                // 检查是否已存在持久化画布（防止重复创建）
                GameObject existingPersistentCanvas = GameObject.Find("PersistentCanvas");
                if (existingPersistentCanvas != null && existingPersistentCanvas.transform.parent == transform)
                {
                    _persistentUIRoot = existingPersistentCanvas.transform;
                    return _persistentUIRoot;
                }

                // 创建新的持久化UI根节点
                GameObject persistentRoot = new GameObject("PersistentCanvas");
            
                // 设置为UIManager的子对象
                persistentRoot.transform.SetParent(transform);
                persistentRoot.transform.localPosition = Vector3.zero;
                persistentRoot.transform.localRotation = Quaternion.identity;
                persistentRoot.transform.localScale = Vector3.one;
            
                // 添加必要的UI组件
                Canvas canvas = persistentRoot.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
                CanvasScaler scaler = persistentRoot.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080); // 根据项目需要调整
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            
                persistentRoot.AddComponent<GraphicRaycaster>();
            
                // 确保画布渲染顺序高于普通UI
                canvas.sortingOrder = 100;
            
                // 标记为DontDestroyOnLoad
                DontDestroyOnLoad(persistentRoot);
            
                _persistentUIRoot = persistentRoot.transform;
            
                Debug.Log("Persistent UI Canvas created and initialized");
            }
            return _persistentUIRoot;
        }
    }

    public UIDatas uiDatas;

    protected override void Awake()
    {
        base.Awake();
        InitDicts();
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
    /// 打开UI面板
    /// </summary>
    /// <param name="name">面板名称</param>
    /// <param name="asPersistent">是否作为持久化面板</param>
    /// <returns>打开的UI面板脚本</returns>
    public BasePanel OpenPanel(string name, bool asPersistent = false)
    {
        BasePanel panel = null;

        // 检查面板是否已经打开
        if (_panelDict.TryGetValue(name, out panel))
        {
            Debug.LogWarning($"面板 {name} 已经打开");
            return panel;
        }

        // 检查面板路径是否存在于路径字典中
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

        // 选择正确的父节点
        Transform parent = asPersistent ? PersistentUIRoot : UIRoot;
        if (parent == null)
        {
            Debug.LogError("无法找到有效的UI根节点");
            return null;
        }

        // 实例化面板
        GameObject panelObj = Instantiate(panelPrefab, parent, false);
        panel = panelObj.GetComponent<BasePanel>();

        if (panel == null)
        {
            Debug.LogError($"面板 {name} 的脚本未挂载或未继承 BasePanel");
            Destroy(panelObj);
            return null;
        }

        panel.OpenPanel(name);
        _panelDict.Add(name, panel);

        // 如果是持久化面板，标记DontDestroyOnLoad
        if (asPersistent)
        {
            DontDestroyOnLoad(panelObj);
        }

        return panel;
    }

    /// <summary>
    /// 关闭UI面板
    /// </summary>
    /// <param name="name">面板名称</param>
    /// <param name="destroyPersistent">是否销毁持久化面板</param>
    /// <returns>是否关闭成功</returns>
    public bool ClosePanel(string name, bool destroyPersistent = false)
    {
        if (!_panelDict.TryGetValue(name, out BasePanel panel))
        {
            Debug.LogWarning($"面板 {name} 当前未打开，无法关闭");
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
    /// 关闭所有非持久化面板
    /// </summary>
    public void CloseAllNonPersistentPanels()
    {
        List<string> panelsToClose = new List<string>();

        foreach (var panel in _panelDict)
        {
            if (!panel.Value.transform.IsChildOf(PersistentUIRoot))
            {
                panelsToClose.Add(panel.Key);
            }
        }

        foreach (string panelName in panelsToClose)
        {
            ClosePanel(panelName);
        }
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