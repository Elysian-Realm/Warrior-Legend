using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float> OnEventRaised;

    public void FadeIn(float duration)
    {
        RaiseEvent(Color.black, duration);
    }

    public void FadeOut(float duration)
    {
        RaiseEvent(Color.clear, duration);
    }

    public void RaiseEvent(Color target, float duration)
    {
        OnEventRaised?.Invoke(target, duration);
    }
}