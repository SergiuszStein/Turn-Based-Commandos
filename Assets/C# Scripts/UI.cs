using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private List<GameObject> _bulletSlots;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PickupSpawner _pickupSpawner;
    [SerializeField] private GameObject _turnTime;
    [SerializeField] private GameObject _ammoCounter;
    [SerializeField] private GameObject _screenHealthBar;
    private TextMeshProUGUI _turnTimeText;
    [SerializeField] private float _timePerTurn = 10;
    [SerializeField] private float _timeBetweenTurns = 5;
    private float _timeLeftInTurn;
    private Image _screenHealthBarImage;
    public bool _playerTurn = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        _turnTimeText = _turnTime.GetComponent<TextMeshProUGUI>();
        _timeLeftInTurn = _timeBetweenTurns;
        _screenHealthBarImage = _screenHealthBar.GetComponent<Image>();
    }

    private void Update()
    {
        Update_Timer();
    }

    private void Update_Timer()
    {
        if (_timeLeftInTurn > 0)
        {
            _timeLeftInTurn -= Time.deltaTime;
        }
        else
        {
            if (_playerTurn)
            {
                _playerManager.NextTurn();
            }
            else
            {
                _playerManager.NextTurnConfirm();
            }
        }

        _turnTimeText.text = _timeLeftInTurn.ToString("f0");
    }

    public void Start_TimeBetweenTurns()
    {
        _playerTurn = false;

        _timeLeftInTurn = _timeBetweenTurns;
    }

    public void Start_TimeInTurn()
    {
        _playerTurn = true;

        _timeLeftInTurn = _timePerTurn;
    }

    public void Update_MainHealthBar(float currentHealth, float fullHealth)
    {
        _screenHealthBarImage.fillAmount = currentHealth / fullHealth;
    }

    public void Update_AmmoDisplay(int count)
    {
        for (int i = 0; i < _bulletSlots.Count; i++)
        {
            _bulletSlots[i].SetActive(i < count);
        }
    }
}
