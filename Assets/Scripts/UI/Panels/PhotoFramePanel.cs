using UnityEngine;
using UnityEngine.UI;

public class PhotoFramePanel : SlidePanel
{
    [Header("组件配置")] [SerializeField] private Image[] photos;
    [SerializeField] private Button closeBtn;


    private void Start()
    {
        closeBtn.onClick.AddListener(ClosePanel);

        InitUI();
    }

    private void InitUI()
    {
        foreach (var photo in photos)
        {
            photo.gameObject.SetActive(false);
        }
        
        if (LevelManager.Instance.IsLevelPlayed(SceneName.Level1_3.ToString()))
        {
            photos[0].gameObject.SetActive(true);
        }

        if (LevelManager.Instance.IsLevelPlayed(SceneName.Level2_3.ToString()))
        {
            photos[1].gameObject.SetActive(true);
        }

        if (LevelManager.Instance.IsLevelPlayed(SceneName.Level3_3.ToString()))
        {
            photos[2].gameObject.SetActive(true);
        }
    }
}