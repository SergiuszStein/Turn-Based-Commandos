using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private GameObject _healSplash;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponent<Player>().Heal(40);

            Instantiate(_healSplash, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
        }
    }
}
