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
        int index = 0;

        foreach (var photo in photos)
        {
            LevelData requiredLevel = LevelManager.Instance.levels[index];

            if (requiredLevel != null && requiredLevel.isUnlocked && requiredLevel.isPlayed)
            {
                photo.gameObject.SetActive(true);
                index++;
            }
            else
            {
                photo.gameObject.SetActive(false);
                return;
            }
        }
    }
}