using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Transform playerTrans;
    private SpriteRenderer playerRenderer;
    private PlayerController playerController;
    public Vector2 firstPosition;
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;

    [Header("事件监听")]
    public SceneLoadEventSO sceneLoadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("广播")]
    public VoidEventSO newGameLaterEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEventSO;
    public SceneLoadEventSO sceneUnloadEventSO;

    private GameSceneSO currentScene;
    private GameSceneSO sceneToLoad;
    private Vector2 posToGo;
    private bool fadeScreen;
    private bool isLoading;
    public float fadeDuration;

    private void Awake()
    {
        sceneLoadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
        newGameEvent.OnEventRaised += NewGame;
        playerRenderer = playerTrans.GetComponent<SpriteRenderer>();
        playerController = playerTrans.GetComponent<PlayerController>();
    }

    private void OnBackToMenuEvent()
    {
        sceneLoadEventSO.RaiseLoadRequestEvent(menuScene, Vector2.zero, true);
    }

    private void Start()
    {
        menuScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        currentScene = menuScene;
        playerTrans.position = Vector3.zero;
        playerRenderer.enabled = false;
        playerController.inputControl.Disable();
    }

    private void NewGame()
    {
        sceneLoadEventSO.RaiseLoadRequestEvent(firstLoadScene, firstPosition, true);
        Invoke("NewGameLater", 1.5f);
    }

    private void NewGameLater()
    {
        newGameLaterEvent.RaiseEvent();
    }

    private void OnLoadRequestEvent(GameSceneSO sceneToLoad, Vector2 posToGo, bool fadeScreen)
    {
        if (isLoading) return;
        isLoading = true;
        this.sceneToLoad = sceneToLoad;
        this.posToGo = posToGo;
        this.fadeScreen = fadeScreen;

        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        playerController.inputControl.Disable();

        if (currentScene)
        {
            if (fadeScreen) fadeEventSO.FadeIn(fadeDuration);
            yield return new WaitForSeconds(fadeDuration);
            sceneUnloadEventSO.RaiseLoadRequestEvent(sceneToLoad, posToGo, true);
            playerRenderer.enabled = false;
            yield return currentScene.sceneReference.UnLoadScene();
        }

        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        loadingOption.Completed += OnSceneLoadCompleted;
    }

    private void OnSceneLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currentScene = sceneToLoad;
        playerTrans.position = posToGo;
        StartCoroutine(ShowScene());
        afterSceneLoadedEvent.RaiseEvent();
    }

    private IEnumerator ShowScene()
    {
        yield return new WaitForSeconds(1);
        if (currentScene.sceneType != SceneType.Menu) playerRenderer.enabled = true;
        if (fadeScreen) fadeEventSO.FadeOut(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        if (currentScene.sceneType != SceneType.Menu) playerController.inputControl.Enable();
        isLoading = false;
    }
}
