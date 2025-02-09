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
    [TextArea] public string description;
    public Color BgColor;
    public string sceneName; // 添加场景名称字段
}

public class SleepBlackPanel : BasePanel
{
    public Text _sleepText;
    public Image _displayImage; // 用于显示图片的组件
    public Text titleText;
    public Text descriptionText;

    [SerializeField] public List<SceneChangerInfo> sceneChangerInfoList; // 存储多个场景信息

    public void StartSleepCounting(float duration, string textStr, UnityAction callBack, SceneName sceneName)
    {
        SceneChangerInfo selectedScene = null;
        
        if(sceneName == SceneName.Level1)
            selectedScene = sceneChangerInfoList[0];
        else if(sceneName == SceneName.Level2)
            selectedScene = sceneChangerInfoList[1];
        else if(sceneName == SceneName.Level3)
            selectedScene = sceneChangerInfoList[2];
        else
        {
            if (sceneChangerInfoList != null && sceneChangerInfoList.Count > 0)
            {
                int randomIndex = Random.Range(0, sceneChangerInfoList.Count);
                selectedScene = sceneChangerInfoList[randomIndex];
            }
        }


        // 如果选择了场景，更新 UI 元素
        if (selectedScene != null)
        {
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
