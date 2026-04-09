using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { None, InMainMenu, InGame, Loading }
public enum SceneState { Loading, Loaded, Unloading, Unloaded }

public class GameManager : MonoBehaviour
{
    #region Scenes Names Declaring
    private readonly string MainMenuScene = "MainMenuScene";
    private readonly string LoadingScene = "LoadingScene";
    //private readonly string IngameSceneArena = "Arena";
    private readonly string IngameSceneArena = "Test";

    private Dictionary<string, SceneState> _loadState = new Dictionary<string, SceneState>
    {
        ["MainMenuScene"] = SceneState.Unloaded,
        ["LoadingScene"] = SceneState.Unloaded,
        ["Test"] = SceneState.Unloaded
    };
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

        if (!_currentSceneName.Equals("")) UnloadAwaiter(SceneManager.UnloadSceneAsync(_currentSceneName), _currentSceneName);
        if (_loadState[sceneName] == SceneState.Unloading)
        {
            yield return new WaitUntil(() => _loadState[sceneName] == SceneState.Unloaded);
        }

        _loadState[sceneName] = SceneState.Loading;
        yield return StartCoroutine(StartLoadingScene());

        var loader = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!loader.isDone)
        {
            _progressControler.UpdateProgress(loader.progress);
            yield return null;
        }

        yield return loader;

        _progressControler.UpdateProgress(loader.progress);
        _currentSceneName = sceneName;
        _loadState[sceneName] = SceneState.Loaded;
        StopLoadingScene();

        CheckScene();
    }

    private void CheckScene()
    {
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
        else if (_currentState == GameState.InGame)
        {
            var objects = SceneManager.GetSceneByName(IngameSceneArena).GetRootGameObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name == "Level")
                {
                    objects[i].GetComponent<LevelManager>().BackToMainMenu = () => { OnGameEnd(); };
                    objects[i].GetComponent<LevelManager>().Initialize(IngameSceneArena);
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
    public void OnGameEnd()
    {
        _nextState = GameState.InMainMenu;
        _isStateCanBeChanged = true;
    }

    async public void UnloadAwaiter(AsyncOperation unloadingSceneOperation, string sceneName)
    {
        _loadState[sceneName] = SceneState.Unloading;
        await unloadingSceneOperation;
        Debug.Log($"{sceneName}: {unloadingSceneOperation.isDone}");
        _loadState[sceneName] = SceneState.Unloaded;
    }
}
