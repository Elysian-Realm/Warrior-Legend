using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("组件")]
    public PlayerStateBar playerStateBar;
    public SpriteRenderer playerRender;
    public Button settingsBtn;
    public GameObject pausePenel;
    public Slider volumeSlider;
    public Slider BGMVolumeSlider;
    public Slider FXVolumeSlider;
    public GameObject virtoryPanel;

    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO sceneUnloadEventSO;
    public FloatEventSO syncVolumeEvent;
    public FloatEventSO syncBGMVolumeEvent;
    public FloatEventSO syncFXVolumeEvent;
    public VoidEventSO virtoryEvent;

    [Header("广播")]
    public VoidEventSO pauseEvent;
    public VoidEventSO unpauseEvent;

    private void Awake()
    {
        settingsBtn.onClick.AddListener(TogglePausePanel);
    }

    private void TogglePausePanel()
    {
        if (Time.timeScale > 0.1f) pauseEvent.RaiseEvent();
        else unpauseEvent.RaiseEvent();
        pausePenel.SetActive(!pausePenel.activeInHierarchy);
        Time.timeScale = Time.timeScale > 0.1f ? 0 : 1;
    }

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        sceneUnloadEventSO.LoadRequestEvent += OnUnloadEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
        syncBGMVolumeEvent.OnEventRaised += OnSyncBGMVolumeEvent;
        syncFXVolumeEvent.OnEventRaised += OnSyncFXVolumeEvent;
        virtoryEvent.OnEventRaised += OnVirtoryEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        sceneUnloadEventSO.LoadRequestEvent -= OnUnloadEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
        syncBGMVolumeEvent.OnEventRaised -= OnSyncBGMVolumeEvent;
        syncFXVolumeEvent.OnEventRaised -= OnSyncFXVolumeEvent;
        virtoryEvent.OnEventRaised -= OnVirtoryEvent;
    }

    private void OnVirtoryEvent()
    {
        virtoryPanel.SetActive(true);
    }

    private void OnSyncVolumeEvent(float amount)
    {
        volumeSlider.value = (amount + 80) / 100;
    }

    private void OnSyncBGMVolumeEvent(float amount)
    {
        BGMVolumeSlider.value = (amount + 80) / 100;
    }

    private void OnSyncFXVolumeEvent(float amount)
    {
        FXVolumeSlider.value = (amount + 80) / 100;
    }

    private void OnUnloadEvent(GameSceneSO sceneToLoad, Vector2 arg1, bool arg2)
    {
        if (sceneToLoad.sceneType == SceneType.Menu)
        {
            playerStateBar.gameObject.SetActive(false);
            playerRender.enabled = false;
        }
        else
        {
            playerStateBar.gameObject.SetActive(true);
            playerRender.enabled = true;
        }
    }

    private void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        playerStateBar.OnHealthChange(persentage);
        playerStateBar.OnPowerChange(character);
    }
}
