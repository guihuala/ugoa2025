using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class SpineTransparencyController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    private Material skeletonMaterial;

    void Start()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();

        if (skeletonAnimation != null)
        {
            skeletonMaterial = skeletonAnimation.GetComponent<Renderer>().material;
        }
    }

    public void SetTransparency(float alpha)
    {
        if (skeletonAnimation != null)
        {
            skeletonAnimation.skeleton.SetColor(new Color(1, 1, 1, Mathf.Clamp01(alpha)));
        }
    }

    public void FadeIn(float duration)
    {
        if (skeletonMaterial != null)
        {
            skeletonMaterial.DOFade(1f, duration);
        }
    }

    public void FadeOut(float duration)
    {
        if (skeletonMaterial != null)
        {
            skeletonMaterial.DOFade(0f, duration);
        }
    }
}