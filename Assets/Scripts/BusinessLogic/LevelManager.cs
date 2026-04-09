using Assets.Scripts.BusinessLogic.Awaiter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private LevelData _levelData;
    [SerializeField] private CinemachineConfiner2D _confiner;
    [SerializeField] private CinemachineCamera _cinemachineCam;

    private GameObject Player = null;
    private PlayerSpawner _playerSpawner = null;
    private ActorController _playerController = null;

    private GameObject InGameUI = null;
    private HPBar PlayerHPBar = null;
    private Button ExitButton;

    private GameObject LevelStructure = null;
    private GameObject _entitiesSpawners = null;
    private List<GameObject> Entities = null;
    private GameObject Lights = null;
    private GameObject CameraBoundaries = null;

    public Action BackToMainMenu;
    private bool _isGameQuiting = false;

    private string _levelName;

    private AllSpawnersAwaiter _spawnersAwaiter = null;

    #endregion

    private void Start()
    {

        //GetPlayer();
        //_spawnersAwaiter = new AllSpawnersAwaiter();
        //LoadSpawners();
        //Debug.Log("Start Finished");
    }

    public async void Initialize(string levelName)
    {
        _levelName = levelName;
        _spawnersAwaiter = new AllSpawnersAwaiter();
        //Instantiate<GameObject>(Resources.Load<GameObject>(""));
        //GetLevelData();
        LevelStructure = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TestLevel/LevelStructure"), transform);
        LevelStructure.transform.position = _levelData.StructurePosition;
        _playerSpawner = LevelStructure.transform.GetChild(5).gameObject.GetComponent<PlayerSpawner>();
        _entitiesSpawners = LevelStructure.transform.GetChild(2).gameObject;
        Player = await _playerSpawner.Spawn();
        SetupPlayer();

        InGameUI = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/InGameUI"), transform);
        PlayerHPBar = InGameUI.transform.GetChild(0).GetChild(0).GetComponent<HPBar>();
        PlayerHPBar.Entity = Player;
        PlayerHPBar.Initialize();

        ExitButton = InGameUI.transform.GetChild(1).GetChild(0).GetComponent<Button>();
        ExitButton.onClick.AddListener(QuitGame);

        CameraBoundaries = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TestLevel/CameraBoundaries"), transform);
        CameraBoundaries.transform.position = _levelData.CameraBoundariesPosition;
        _confiner.BoundingShape2D = CameraBoundaries.GetComponent<CompositeCollider2D>();
        _cinemachineCam.Follow = Player.transform;
        await LoadEntities();
        Lights = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TestLevel/Lights"), transform);
        Lights.transform.position = _levelData.LightsPosition;
    }

    private void Update()
    {
        if (_isGameQuiting)
        {
            _isGameQuiting = false;
            BackToMainMenu();
        }
    }

    private void GetLevelData()
    {
        _levelData = Resources.Load<LevelData>($"LevelData/{_levelName}Data");
    }

    private void SetupPlayer()
    {
        _playerController = Player.GetComponent<ActorController>();
        _playerController.OnDeath = QuitGame;
    }

    async private Task LoadEntities()
    {
        var spawners = _entitiesSpawners.transform.GetComponentsInChildren<EntitySpawner>();
        //Debug.Log("spawners "+(spawners == null));
        Entities = await _spawnersAwaiter.AwaitFor(spawners);

        return;
    }

    public void QuitGame() => _isGameQuiting = true;
}
