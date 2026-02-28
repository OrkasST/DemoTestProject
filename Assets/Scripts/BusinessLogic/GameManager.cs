using Mono.Cecil.Cil;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { None, InMainMenu, InGame, Loading }

public class GameManager : MonoBehaviour
{
    #region Scenes Names Declaring
    private readonly string MainMenuScene = "MainMenuScene";
    private readonly string LoadingScene = "LoadingScene";
    //private readonly string IngameSceneArena = "Arena";
    private readonly string IngameSceneArena = "Test";

    #endregion

    private GameState _currentState = GameState.None;
    private GameState _nextState = GameState.InMainMenu;
    private GameState _previousState = GameState.None;

    private string _currentSceneName = "";

    private ProgressController _progressControler;

    private bool _isStateCanBeChanged = false;

    void Start()
    {
        ChangeState();
    }
    void Update()
    {
        if (_isStateCanBeChanged) ChangeState();
    }

    private void ChangeState()
    {
        if (_nextState == GameState.None) return;

        switch (_nextState)
        {
            case GameState.InMainMenu: StartCoroutine(LoadScene(MainMenuScene)); break;
            case GameState.InGame: StartCoroutine(LoadScene(IngameSceneArena)); break;
            default: break;
        }
    }

    private IEnumerator LoadScene(string sceneName)
    {
        if (_currentState == GameState.Loading) yield return null;
        _isStateCanBeChanged = false;
        if (_currentSceneName != "") SceneManager.UnloadSceneAsync(_currentSceneName);

        yield return StartCoroutine(StartLoadingScene());

        var loader = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (!loader.isDone)
        {
            _progressControler.UpdateProgress(loader.progress);
            yield return null;
        }

        yield return loader;

        _progressControler.UpdateProgress(loader.progress);
        _currentSceneName = sceneName;
        StopLoadingScene();

        CheckScene();
    }

    private void CheckScene()
    {
        Debug.Log(_currentState);
        if (_currentState == GameState.InMainMenu)
        {
            var objects = SceneManager.GetSceneByName(MainMenuScene).GetRootGameObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name == "MenuCanvas")
                {
                    objects[i].GetComponent<GameLoader>().OnClick = () => { OnGameStart(); };
                    break;
                }
            }
        }
    }

    private IEnumerator StartLoadingScene()
    {
        _previousState = _currentState;
        _currentState = GameState.None;

        AsyncOperation loading = SceneManager.LoadSceneAsync(LoadingScene, LoadSceneMode.Additive);
        yield return loading;

        _currentState = GameState.Loading;

        var objects = SceneManager.GetSceneByName(LoadingScene).GetRootGameObjects();

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].name == "LoadingCanvas")
            {
                _progressControler = objects[i].transform.GetChild(1).GetComponent<Slider>().GetComponent<ProgressController>();
                break;
            }
        }
    }
    async private void StopLoadingScene()
    {
        _currentState = _nextState;
        _nextState = GameState.None;
        _isStateCanBeChanged = false;
        await SceneManager.UnloadSceneAsync(LoadingScene);
    }

    public void OnGameStart()
    {
        _nextState = GameState.InGame;
        _isStateCanBeChanged = true;
    }
}
