using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class SceneChangerInfo
{
    public Sprite sceneImage;
    public string title;
    [TextArea]public string description;
    public Color BgColor;
}

public class SleepBlackPanel : BasePanel
{
    private Text _sleepText;
    private Image _displayImage; // 用于显示图片的组件
    private Text titleText;
    private Text descriptionText;

    [SerializeField] public List<SceneChangerInfo> sceneChangerInfoList; // 存储多个场景信息
    
    protected override void Awake()
    {
        base.Awake();

        titleText = transform.Find("Title").GetComponent<Text>();
        descriptionText = transform.Find("Description").GetComponent<Text>();
        _sleepText = transform.Find("SleepText").GetComponent<Text>();
        _displayImage = transform.Find("DisplayImage").GetComponent<Image>();
    }

    public void StartSleepCounting(float duration, string textStr, UnityAction callBack)
    {
        if (sceneChangerInfoList != null && sceneChangerInfoList.Count > 0)
        {
            int randomIndex = Random.Range(0, sceneChangerInfoList.Count);
            SceneChangerInfo selectedScene = sceneChangerInfoList[randomIndex];
            
            _displayImage.sprite = selectedScene.sceneImage;
            _displayImage.gameObject.SetActive(true);
            
            transform.GetComponent<Image>().color = selectedScene.BgColor;
            
            titleText.text = selectedScene.title;
            descriptionText.text = selectedScene.description;
        }

        // 动画播放文字效果
        DOTween.Sequence()
            .Append(_sleepText.DOText(textStr, duration)
                .OnComplete(() =>
                {
                    _displayImage.gameObject.SetActive(false);
                    callBack?.Invoke();
                }));
    }
}