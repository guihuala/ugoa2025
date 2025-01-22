using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Ë¯¾õºÚÆÁÃæ°å
/// </summary>
public class SleepBlackPanel : BasePanel
{

    private Text _sleepText;

    protected override void Awake()
    {
        base.Awake();

        _sleepText = transform.GetChild(0).GetComponent<Text>();
    }
    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
    }

    public void StartSleepCounting(float duration,string textStr,UnityAction callBack)
    {
        DOTween.Sequence().Append(_sleepText.DOText(textStr, duration).OnComplete(() =>
        {
            callBack?.Invoke();
        }));
    }
}
