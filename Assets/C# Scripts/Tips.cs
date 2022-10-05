using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tips : MonoBehaviour
{
    [SerializeField] private List<String> _tips;
    [SerializeField, Range(1, 10)] private float _timer;
    private float _timerLeft;
    private int _tipRotation = 0;
    [SerializeField] private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        Change_Tip();
    }

    private void Update()
    {
        if (_timerLeft <= 0)
        {
            Change_Tip();

            _timerLeft = _timer;
        }
        else
        {
            _timerLeft -= Time.deltaTime;
        }
    }

    private void Change_Tip()
    {
        _textMesh.text = "Tip: " + _tips[_tipRotation];
        
        _tipRotation++;

        _tipRotation %= _tips.Count;
    }
}
