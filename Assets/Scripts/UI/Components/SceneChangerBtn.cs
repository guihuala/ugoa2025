using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangerBtn : MonoBehaviour
{
    [SerializeField] private SceneName sceneName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener((() => { PressSelection(sceneName); }));
    }

    public void PressSelection(SceneName _LevelName)
    {
        SceneLoader.Instance.LoadScene(_LevelName, "...");
    }
}
