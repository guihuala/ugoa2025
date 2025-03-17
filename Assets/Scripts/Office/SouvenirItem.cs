using UnityEngine;

public class SouvenirItem : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
        
        if(SaveManager.Instance.isComplete)
            gameObject.SetActive(true);
    }
}
