using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector2, bool> LoadRequestEvent;

    public void RaiseLoadRequestEvent(GameSceneSO locationToLoad, Vector2 posToGo, bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(locationToLoad, posToGo, fadeScreen);
    }
}
