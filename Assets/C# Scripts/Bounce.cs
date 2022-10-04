using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    [SerializeField] private GameObject _uI;

    private void Awake()
    {
        _uI = GameObject.Find("UI");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && _uI.GetComponent<UI>()._playerTurn)
        {
            float _jump = other.gameObject.GetComponent<Player>()._jumpForce;
            
            other.gameObject.GetComponent<Player>()._rigidbody.AddForce(new Vector3(0, (_jump * 1.5f), 0));
        }
    }
}
