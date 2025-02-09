using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour,IEnterSpecialItem
{
    public void Apply()
    {
        SceneLoader.Instance.LoadScene(SceneName.LevelSelection,"...");
    }
}
