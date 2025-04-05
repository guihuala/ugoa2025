using UnityEngine;
using UnityEngine.UI;

public class TutorialBtn : MonoBehaviour
{
    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        
        btn.onClick.AddListener(ClickButton);
    }

    private void ClickButton()
    {
        FindObjectOfType<GuideMask>().OnClickButton();
    }
}
