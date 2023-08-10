using UnityEngine.Events;

public interface IInteractable
{
    void TriggerAction(UnityAction callback = null);
}
