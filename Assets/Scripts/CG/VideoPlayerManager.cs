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
        videoPlayer.clip = videoClip;
        
        videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        rawImage.texture = videoPlayer.targetTexture;
        
        videoPlayer.Play();
        
        videoPlayer.loopPointReached += OnVideoEnd;
        
        skipButton.onClick.AddListener(SkipVideo);
    }

    /// <summary>
    /// 视频播放结束时触发
    /// </summary>
    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneLoader.Instance.LoadScene(SceneName.LevelSelection, "...");
    }

    /// <summary>
    /// 跳过视频并加载指定场景
    /// </summary>
    private void SkipVideo()
    {
        videoPlayer.Stop();
        
        SceneLoader.Instance.LoadScene(SceneName.LevelSelection, "...");
    }
}
