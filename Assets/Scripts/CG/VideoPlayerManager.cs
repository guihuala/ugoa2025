using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoPlayerManager : MonoBehaviour
{
    [Header("视频播放器")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;

    [Header("视频文件")]
    [SerializeField] private VideoClip videoClip;

    [Header("跳过按钮")]
    [SerializeField] private Button skipButton; // 跳过按钮

    [Header("场景控制")]
    [SerializeField] private string[] disableSkipSceneNames; // 需要禁用跳过按钮的场景名列表

    private float lastInputTime = 0f; // 记录上次输入时间
    private float hideDelay = 3f; // 多少秒后隐藏按钮
    private Vector3 lastMousePosition; // 记录上次鼠标位置
    private bool isButtonVisible = false; // 按钮当前是否可见
    private string previousSceneName; // 上一个场景的名字

    private void Start()
    {
        // 获取上一个场景的名字
        previousSceneName = PlayerPrefs.GetString("LastSceneName", "");
        
        videoPlayer.clip = videoClip;
        videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        rawImage.texture = videoPlayer.targetTexture;

        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;

        skipButton.onClick.AddListener(SkipVideo);

        // 根据上一个场景决定是否禁用跳过按钮
        bool shouldDisableSkip = CheckIfDisableSkip();
        skipButton.interactable = !shouldDisableSkip; // 禁用按钮交互（但按钮仍可见）
        skipButton.gameObject.SetActive(false); // 初始隐藏跳过按钮
        
        lastMousePosition = Input.mousePosition; // 记录初始鼠标位置
    }

    /// <summary>
    /// 检查是否需要禁用跳过按钮
    /// </summary>
    private bool CheckIfDisableSkip()
    {
        if (disableSkipSceneNames == null || disableSkipSceneNames.Length == 0)
            return false;

        foreach (string sceneName in disableSkipSceneNames)
        {
            if (previousSceneName == sceneName)
            {
                return true; // 如果上一个场景在禁用列表中，返回 true
            }
        }
        return false;
    }

    private void Update()
    {
        // 检测鼠标移动、键盘输入 或 触摸输入（移动端）
        if (Input.anyKeyDown || MouseMoved() || TouchDetected())
        {
            ShowSkipButton();
            lastInputTime = Time.time; // 更新上次输入时间
        }

        // 一定时间无输入后隐藏按钮
        if (isButtonVisible && Time.time - lastInputTime > hideDelay)
        {
            HideSkipButton();
        }
    }

    /// <summary>
    /// 检测鼠标是否移动
    /// </summary>
    private bool MouseMoved()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 检测触摸输入（移动端）
    /// </summary>
    private bool TouchDetected()
    {
        return Input.touchCount > 0; // 只要有手指触摸屏幕，就返回 true
    }

    /// <summary>
    /// 显示跳过按钮
    /// </summary>
    private void ShowSkipButton()
    {
        if (!isButtonVisible)
        {
            skipButton.gameObject.SetActive(true);
            isButtonVisible = true;
        }
    }

    /// <summary>
    /// 隐藏跳过按钮
    /// </summary>
    private void HideSkipButton()
    {
        if (isButtonVisible)
        {
            skipButton.gameObject.SetActive(false);
            isButtonVisible = false;
        }
    }

    /// <summary>
    /// 视频播放结束时触发
    /// </summary>
    private void OnVideoEnd(VideoPlayer vp)
    {
        UIManager.Instance.OpenPanel("CompletePanel");
    }

    /// <summary>
    /// 跳过视频并加载指定场景
    /// </summary>
    private void SkipVideo()
    {
        videoPlayer.Stop();
        UIManager.Instance.OpenPanel("CompletePanel");
    }
}