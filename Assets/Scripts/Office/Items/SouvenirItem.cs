using UnityEngine;

public class SouvenirItem : BaseOfficeItem
{
    protected override void Start()
    {
        gameObject.SetActive(false);
        
        if(SaveManager.Instance.isComplete)
            gameObject.SetActive(true);
    }
}
