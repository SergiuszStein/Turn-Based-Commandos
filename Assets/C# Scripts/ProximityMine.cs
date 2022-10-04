using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProximityMine : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _meshes;
    [SerializeField, Range(0, 10)] private float _mineTimerMax;
    [SerializeField, Range(0, 10)] private float _boomRadius;
    [SerializeField, Range(0, 100)] private int _mineDamage;
    [SerializeField] private Material _red;
    [SerializeField] private Material _gray;
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _boomModel;
    private bool _mineLightOn = false;
    private float _mineLightTimer = 0.2f;
    private float _mineLightTimeLeft;
    private float _mineTimerLeft;
    private bool _mineActivated = false;

    private void Awake()
    {
        _mineTimerLeft = _mineTimerMax;
        _mineLightTimeLeft = _mineLightTimer;
        _model.SetActive(true);
    }

    private void Update()
    {
        if (_mineActivated)
        {
            _mineTimerLeft -= Time.deltaTime;

            if (_mineTimerLeft <= 0)
            {
                Explode();
            }
        }

        Update_Material();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _mineActivated = true;
        }
    }

    private void Explode()
    {
        Collider[] _hitColliders = Physics.OverlapSphere(transform.position, _boomRadius);

        for (int i = 0; i < _hitColliders.Length; i++)
        {
            GameObject _hit = _hitColliders[i].gameObject;
            
            if (_hit.layer == LayerMask.NameToLayer("Player"))
            {
                _hit.GetComponent<Player>().Damage(_mineDamage);
            }
            else if (_hit.layer == LayerMask.NameToLayer("GasBarrel"))
            {
                _hit.GetComponent<GasBarrel>().Damage();
            }
        }

        Instantiate(_boomModel, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }

    private void Update_Material()
    {
        if (_mineActivated)
        {
            if (_mineLightTimeLeft <= 0)
            { 
                if (_mineLightOn)
                { 
                    for (int i = 0; i < _meshes.Count; i++)
                    { 
                        _meshes[i].material = _gray;
                        
                        _mineLightOn = false;
                    }
                }
                else 
                { 
                    for (int i = 0; i < _meshes.Count; i++)
                    { 
                        _meshes[i].material = _red;
                        
                        _mineLightOn = true;
                    }
                }
                
                _mineLightTimeLeft = _mineLightTimer;
            }
            else 
            { 
                _mineLightTimeLeft -= Time.deltaTime;
            }
        }
        
    }
}
