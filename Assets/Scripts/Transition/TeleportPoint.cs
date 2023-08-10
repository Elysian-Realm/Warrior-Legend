using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO sceneLoadEventSO;
    public GameSceneSO sceneToGo;
    public Vector2 positionToGo;
    public void TriggerAction(UnityAction callback = null)
    {
        StopAllCoroutines();
        sceneLoadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
        callback?.Invoke();
    }
}
