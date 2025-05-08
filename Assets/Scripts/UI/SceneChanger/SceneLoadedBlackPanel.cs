using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadedBlackPanel : BasePanel
{
    public override void OpenPanel(string name)
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        panelName = name;

        canvasGroup.alpha = 1;

        gameObject.SetActive(true);
    }
}