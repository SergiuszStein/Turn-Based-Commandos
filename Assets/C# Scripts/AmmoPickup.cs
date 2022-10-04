using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private GameObject _smokePoof;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponent<Player>().Arm(1);
            
            Instantiate(_smokePoof, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
        }
    }
}
