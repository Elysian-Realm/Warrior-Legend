using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chest : MonoBehaviour, IInteractable
{
    private AudioDefination audioDefination;
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;

    [Header("广播")]
    public VoidEventSO victoryEvent;

    private void Awake()
    {
        audioDefination = GetComponent<AudioDefination>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;
    }

    public void TriggerAction(UnityAction callback)
    {
        OpenChest();
        audioDefination.PlayAudioClip();
        victoryEvent.RaiseEvent();
        callback?.Invoke();
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = openSprite;
        isDone = true;
        this.gameObject.tag = "Untagged";
    }
}
