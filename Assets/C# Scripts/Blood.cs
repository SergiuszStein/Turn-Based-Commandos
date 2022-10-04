using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float _splaterLife;
    private float _splaterLifeLeft;

    private void Awake()
    {
        _splaterLifeLeft = _splaterLife;
    }

    private void Update()
    {
        if (_splaterLifeLeft <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            _splaterLifeLeft -= Time.deltaTime;
        }
    }
}
