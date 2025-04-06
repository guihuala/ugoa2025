using UnityEngine;
using UnityEngine.UI;

public class TutorialBtn : MonoBehaviour
{
    private Button btn;
    [SerializeField] private BtnType btnType;
    
    private enum BtnType
    {
        None,
        ChangeScene,
        UI
    }

    private void Start()
    {
        btn = GetComponent<Button>();
        
        btn.onClick.AddListener(ClickButton);
    }

    private void ClickButton()
    {
        FindObjectOfType<GuideMask>().OnClickButton();
        ButtonTypeEffect();
    }

    private void ButtonTypeEffect()
    {
        switch (btnType)
        {
            case BtnType.None:
                break;
            case BtnType.ChangeScene:
                SceneLoader.Instance.LoadScene(SceneName.OfficeScene,"...");
                break;
            case BtnType.UI:
                UIManager.Instance.OpenPanel("MailboxPanel");
                break;
        }
    }
}
