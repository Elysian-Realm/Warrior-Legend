using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    public Image fadeImage;

    [Header("事件监听")]
    public FadeEventSO fadeEventSO;

    private void OnEnable()
    {
        fadeEventSO.OnEventRaised += OnFadeEvent;
    }

    private void OnDisable()
    {
        fadeEventSO.OnEventRaised -= OnFadeEvent;
    }

    private void OnFadeEvent(Color target, float duration)
    {
        fadeImage.DOBlendableColor(target, duration);
    }
}
