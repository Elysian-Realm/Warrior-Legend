using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("事件监听")]
    public PlayAudioEventSO FXEvent;
    public PlayAudioEventSO BGMEvent;
    public FloatEventSO volumeEvent;
    public FloatEventSO BGMVolumeEvent;
    public FloatEventSO FXVolumeEvent;
    public VoidEventSO pauseEvent;

    [Header("广播")]
    public FloatEventSO syncVolumeEvent;
    public FloatEventSO syncBGMVolumeEvent;
    public FloatEventSO syncFXVolumeEvent;

    [Header("组件")]
    public AudioSource BGMSource;
    public AudioSource FXSource;
    public AudioMixer audioMixer;

    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        volumeEvent.OnEventRaised += OnVolumeChange;
        BGMVolumeEvent.OnEventRaised += OnBGMVolumeChange;
        FXVolumeEvent.OnEventRaised += OnFXVolumeChange;
        pauseEvent.OnEventRaised += OnPauseEvent;
    }

    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
        volumeEvent.OnEventRaised -= OnVolumeChange;
        BGMVolumeEvent.OnEventRaised -= OnBGMVolumeChange;
        FXVolumeEvent.OnEventRaised -= OnFXVolumeChange;
        pauseEvent.OnEventRaised -= OnPauseEvent;
    }

    private void OnPauseEvent()
    {
        float amount;
        audioMixer.GetFloat("MasterVolume", out amount);
        syncVolumeEvent.RaiseEvent(amount);
        audioMixer.GetFloat("BGMVolume", out amount);
        syncBGMVolumeEvent.RaiseEvent(amount);
        audioMixer.GetFloat("FXVolume", out amount);
        syncFXVolumeEvent.RaiseEvent(amount);
    }

    private void OnVolumeChange(float amount)
    {
        audioMixer.SetFloat("MasterVolume", amount * 100 - 80);
    }
    private void OnBGMVolumeChange(float amount)
    {
        audioMixer.SetFloat("BGMVolume", amount * 100 - 80);
    }
    private void OnFXVolumeChange(float amount)
    {
        audioMixer.SetFloat("FXVolume", amount * 100 - 80);
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }

    private void OnFXEvent(AudioClip clip)
    {
        FXSource.clip = clip;
        FXSource.Play();
    }
}
