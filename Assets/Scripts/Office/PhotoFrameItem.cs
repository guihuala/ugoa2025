using UnityEngine;

public class PhotoFrameItem : MonoBehaviour
{
    public GameObject[] photos;

    private void Start()
    {
        InitUI();
    }
    
    private void InitUI()
    {
        int index = 0;

        foreach (var photo in photos)
        {
            if (index >= LevelManager.Instance.levels.Count)
            {
                photo.SetActive(false);
                continue;
            }
            
            LevelData requiredLevel = LevelManager.Instance.levels[index * 4];
            
            if (requiredLevel != null && requiredLevel.isUnlocked && requiredLevel.isPlayed)
            {
                photo.SetActive(true);
                index++;
            }
            else
            {
                photo.SetActive(false);
                return;
            }
        }
    }
}
