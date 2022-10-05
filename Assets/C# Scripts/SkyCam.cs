using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCam : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _rotate;
    [SerializeField] private GameObject _zoom;
    [SerializeField] private GameObject _angle;
    [SerializeField] private GameObject _cameraPosition;
    [SerializeField] private UI _ui;
    [SerializeField, Range(1, 100)] private float _rotationSpeed;

    // Update is called once per frame

    void Update()
    {
        Position_Camera();
        Rotate_Rotate();
    }

    private void Position_Camera()
    {
        if (!_ui._playerTurn)
        {
            _camera.transform.position = _cameraPosition.transform.position;
            _camera.transform.rotation = _cameraPosition.transform.rotation;
        }
    }

    private void Rotate_Rotate()
    {
        _rotate.transform.Rotate(new Vector3(0, (_rotationSpeed * Time.deltaTime), 0), Space.Self);
    }
}
