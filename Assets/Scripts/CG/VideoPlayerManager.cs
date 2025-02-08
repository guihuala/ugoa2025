using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    [Header("视频播放器")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;

    [Header("视频文件")]
    [SerializeField] private VideoClip videoClip;

    [Header("跳过按钮")]
    [SerializeField] private Button skipButton; // 跳过按钮

    private void Start()
    {
        // 设置视频文件
        videoPlayer.clip = videoClip;

        // 设置视频显示的 RawImage
        videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        rawImage.texture = videoPlayer.targetTexture;

        // 播放视频
        videoPlayer.Play();

        // 监听视频播放完毕事件
        videoPlayer.loopPointReached += OnVideoEnd;

        // 添加跳过按钮的监听
        skipButton.onClick.AddListener(SkipVideo);
    }

    /// <summary>
    /// 视频播放结束时触发
    /// </summary>
    private void OnVideoEnd(VideoPlayer vp)
    {
        // 视频播放完毕后，跳转到指定的场景
        SceneLoader.Instance.LoadScene(SceneName.LevelSelection, "...");
    }

    /// <summary>
    /// 跳过视频并加载指定场景
    /// </summary>
    private void SkipVideo()
    {
        // 停止视频播放
        videoPlayer.Stop();

        // 跳转到指定场景
        SceneLoader.Instance.LoadScene(SceneName.LevelSelection, "...");
    }
}
