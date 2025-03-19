using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletePanel : BasePanel
{
    [SerializeField] private Button mainMenuBtn;

    private void Start()
    {
        mainMenuBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ClosePanel(panelName);

            SceneLoader.Instance.LoadScene(SceneName.LevelSelection, "...");
        });
    }
}