using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { None, InMainMenu, InGame, Loading }

public class NewMonoBehaviourScript : MonoBehaviour
{
    #region Scenes Names Declaring
    private readonly string MainMenuScene = "MainMenuScene";
    private readonly string LoadingScene = "LoadingScene";
    private readonly string IngameSceneArena = "Arena";
    #endregion

    private GameState _currentState = GameState.None;
    private GameState _nextState = GameState.InMainMenu;
    private GameState _previousState = GameState.None;

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
            case GameState.InMainMenu: LoadScene(MainMenuScene); break;
            default: break;
        }
    }

    async private void LoadScene(string sceneName)
    {
        if (_currentState == GameState.Loading) return;

        Debug.Log("Loading " + sceneName);
        StartLoadingScene();
        await Task.Delay(2000);
        await SceneManager.LoadSceneAsync(sceneName);
        StopLoadingScene();
    }

    private void StartLoadingScene()
    {
        _previousState = _currentState;
        _currentState = GameState.Loading;
        SceneManager.LoadScene(LoadingScene);
        Debug.Log("Loading Started");
    }
    private void StopLoadingScene()
    {
        _currentState = _nextState;
        _nextState = GameState.None;
        Debug.Log("Loading Ended");
    }
}
