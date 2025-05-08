using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlackPanel : BasePanel
{

    private Text _sleepText;

    protected override void Awake()
    {
        base.Awake();

        _sleepText = transform.GetChild(0).GetComponent<Text>();
    }
    
    public void StartCounting(float duration,UnityAction callBack)
    {
        DOTween.Sequence().Append(_sleepText.DOText("...", duration).OnComplete(() =>
        {
            callBack?.Invoke();
        }));
    }
}