using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BushShakeTrigger : MonoBehaviour,IShootable
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        ShakeBush();
    }

    private void ShakeBush()
    {
        transform.DOShakeRotation(0.5f, new Vector3(0, 5f, 5f), 10, 90, false)
            .OnComplete(() => StopShaking());

        transform.DOShakePosition(0.5f, 0.2f, 8, 90, false, true);
    }

    private void StopShaking()
    {
        transform.DOLocalMove(originalPosition, 0.3f).SetEase(Ease.OutQuad);

        transform.DOLocalRotateQuaternion(originalRotation, 0.3f).SetEase(Ease.OutQuad);
    }

    public void OnShot(BulletLifecycle bullet)
    {
        ShakeBush();
        
        int index = Random.Range(1, 3);
        AudioManager.Instance.PlaySfx("into_grass_"+ index);
    }
}