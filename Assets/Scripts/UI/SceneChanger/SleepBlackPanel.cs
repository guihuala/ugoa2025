using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SleepBlackPanel : BasePanel
{
    private Text _sleepText;
    [SerializeField] private List<Sprite> randomImages; // 图片列表
    private Image _displayImage; // 用于显示图片的组件

    protected override void Awake()
    {
        base.Awake();

        _sleepText = transform.GetChild(0).GetComponent<Text>();
        _displayImage = transform.GetChild(1).GetComponent<Image>();
    }

    public void StartSleepCounting(float duration, string textStr, UnityAction callBack)
    {
        // 随机选择一张图片
        if (randomImages != null && randomImages.Count > 0)
        {
            int randomIndex = Random.Range(0, randomImages.Count);
            _displayImage.sprite = randomImages[randomIndex];
            _displayImage.gameObject.SetActive(true);
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