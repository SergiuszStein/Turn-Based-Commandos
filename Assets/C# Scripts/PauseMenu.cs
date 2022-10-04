using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private Slider _slider;
    private bool _optionsOpen = false;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && !_optionsOpen)
        {
            _slider.value = _playerManager._activePlayer._3pCameraSensitivity;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            _container.SetActive(true);
            _optionsOpen = true;
        }
        else if (Input.GetButtonDown("Cancel") && _optionsOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            _container.SetActive(false);
            _optionsOpen = false;
        }
    }

    public void Apply_MouseSensitivity(float mouseSensitivity)
    {
        _playerManager._activePlayer._3pCameraSensitivity = mouseSensitivity;
    }
}
