using System;
using System.Collections.Generic;
using C__Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> _players;
    [SerializeField] public Player _activePlayer;
    [SerializeField] private PickupSpawner _pickupSpawner;
    [SerializeField] private int _playerCount;
    [SerializeField] private int _currentPlayerCount;
    [SerializeField] private int _currentPlayerTurn;
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private UI _uI;
    public GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        StartMatch();
    }

    private void Update()
    {
        CheckIfSkipTurn();
    }

    private void StartMatch()
    {
        _playerCount = _gameManager._playerNumber - 1;
        _currentPlayerCount = 3;
        _currentPlayerTurn = _playerCount;
        _activePlayer = _players[_currentPlayerTurn];

        for (int i = 0; i < _players.Count; i++)
        {
            _players[i]._playerIndex = i;
            _players[i].ActivatePlayer(false, false);

            if (!(i <= _playerCount))
            {
                _players[i].LmaoDed();
            }
        }
    }

    private void CheckIfSkipTurn()
    {
        if (Input.GetButtonDown("SkipTurn"))
        {
            NextTurn();
        }
    }
    
    public void NextTurn()
    {
        if (_uI._playerTurn)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].ActivatePlayer(false, false);
            }
            
            _uI.Start_TimeBetweenTurns();
            
            _pickupSpawner.SpawnStuff();
        }
    }
    
    public void NextTurnConfirm()
    {
        if (!_uI._playerTurn)
        {
            _currentPlayerTurn = (_currentPlayerTurn + 1) % (_playerCount + 1);

            if (!_players[_currentPlayerTurn]._isAlive)
            {
                NextTurnConfirm();
                return;
            }
            else
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    _players[i].ActivatePlayer(i == _currentPlayerTurn, true);
                }
            }
            
            _activePlayer = _players[_currentPlayerTurn];
                    
            _uI.Start_TimeInTurn();
        }
    }

    public void PlayerDied(int playerIndex, GameObject player)
    {
        player.GetComponent<Player>()._isAlive = false;

        player.transform.position = new Vector3(0, -5, 0);
            
        _currentPlayerCount = _currentPlayerCount - 1;

        if (_currentPlayerCount <= 0)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i]._isAlive)
                {
                    _gameManager._playerWin = _players[i]._playerColor;
                }
            }
            
            SceneManager.LoadScene(2);
        }
    }
}
