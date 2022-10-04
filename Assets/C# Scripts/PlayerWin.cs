using System.Collections;
using System.Collections.Generic;
using C__Scripts;
using TMPro;
using UnityEngine;

public class PlayerWin : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI _playerWinText;
    
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();

        _playerWinText.text = _gameManager._playerWin + " Player Wins";
    }
}
