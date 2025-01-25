using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenUI : BasePanel
{
    [Header("Black Screen UI")]
    [SerializeField] private Image blackScreenImage; // 黑屏 UI 的 Image 组件
    [SerializeField] private float fadeSpeed = 2f; // 渐变速度
    
}