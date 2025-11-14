using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public Color TeamOneColor { get; private set; }
    [field: SerializeField] public Color TeamTwoColor { get; private set; }


    private Dictionary<PlayerRef, EnemyController> _clients;
    private List<EnemyController> _myPlayers;

    private Dictionary<EnemyController, Color> _teamDictionary;

    [SerializeField] private GameObject _winImage;
    [SerializeField] private GameObject _loseImage;
    //[SerializeField] private TimeManager _timeManager;

    bool _hasInitialized;

    public event Action OnStartGame;


    private void Awake()
    {
        Instance = this;
        _clients = new Dictionary<PlayerRef, EnemyController>();
        _myPlayers = new List<EnemyController>();
        _teamDictionary = new Dictionary<EnemyController, Color>();
    }

    public override void Spawned()
    {
        _hasInitialized = true;

        //OnStartGame += ResetPlayers;
        OnStartGame += DisableGameOver;
    }

    public void AddPlayer(EnemyController player)
    {
        var newPlayer = player.Object.InputAuthority;

        if (_clients.ContainsKey(newPlayer)) return;

        _clients.Add(newPlayer, player);
        _myPlayers.Add(player);

        if (!_hasInitialized) return;

        SetPlayerTeam(player);

        //if (_clients.Count >= 2)
        //{
        //    OnStartGame += _timeManager.RPC_StartCountdown;

        //    OnStartGame?.Invoke();
        //}
    }

    private void SetUpPlayers()
    {
        foreach (var player in _myPlayers)
        {
            SetPlayerTeam(player);
        }

    }

    private void SetIdColor(EnemyController player)
    {

        if (HasStateAuthority)
        {
            _teamDictionary.TryAdd(player, player.HasInputAuthority ? TeamOneColor : TeamTwoColor);
        }
        else
        {
            _teamDictionary.TryAdd(player, player.HasInputAuthority ? TeamTwoColor : TeamOneColor);
        }
    }

    private void SetPlayerTeam(EnemyController player)
    {
        SetIdColor(player);

        var colorRef = _teamDictionary[player];

        player.RPC_SetTeam(colorRef);
        //StartCoroutine(CallSetID(player));
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_GameOver()
    {
        var winnerColor = ColorManager.Instance.GetWinner();

        var winner = _teamDictionary.Keys.First();

        foreach (var pair in _teamDictionary)
        {
            if (pair.Value == winnerColor)
            {
                winner = pair.Key;
            }
        }

        RPC_ShowGameOverImage(winner.Object);
    }

    [Rpc]
    private void RPC_ShowGameOverImage(NetworkObject winner)
    {
        if (winner.InputAuthority == Runner.LocalPlayer)
        {
            _winImage.SetActive(true);
        }
        else
        {
            _loseImage.SetActive(true);
        }
    }

    //private void ResetPlayers()
    //{
    //    foreach (var player in _myPlayers)
    //    {
    //        player.enabled = false;
    //        player.RPC_SetPosition(SpawnerInScene.Instance.GetSpawnPoint(player.SpawnPointIndex).position);
    //    }
    //}

    //[Rpc]
    //public void RPC_StartGame()
    //{
    //    foreach (var player in _myPlayers)
    //    {
    //        player.enabled = true;
    //    }

    //    _timeManager.RPC_StartTimer();
    //}

    private void DisableGameOver()
    {
        _loseImage.SetActive(false);
        _winImage.SetActive(false);
    }

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //public void RPC_RequestRestartGame()
    //{
    //    RPC_RestartGame();
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RPC_RestartGame()
    //{
    //    OnStartGame?.Invoke();
    //}

    //public void OnPlayerGoToMainMenu()
    //{
     
    //    Runner.Shutdown();
    //}

    //public void DespawnPlayer(PlayerRef disconnectedPlayer)
    //{
    //    Runner.Despawn(_clients[disconnectedPlayer].Object);

    //    _myPlayers.Remove(_clients[disconnectedPlayer]);

    //    _clients.Remove(disconnectedPlayer);

    //    if (_myPlayers.Count < 2) OnStartGame -= _timeManager.RPC_StartCountdown;
    //}
}