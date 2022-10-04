using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    [SerializeField, Range(1, 100)] private float _bulletFast;
    [SerializeField, Range(0f, 10)] private float _lifeTime;
    [SerializeField, Range(1, 100)] private int _bulletDamage;

    private void Update()
    {
        Fast();
        Update_LifeTimer();
    }

    private void Fast()
    {
        transform.localPosition += transform.forward * (_bulletFast * Time.deltaTime);
    }

    private void Update_LifeTimer()
    {
        _lifeTime -= Time.deltaTime;
        
        if (_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponent<Player>().Damage(_bulletDamage);
            
            Destroy(this.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("GasBarrel"))
        {
            other.gameObject.GetComponent<GasBarrel>().Damage();
            
            Destroy(this.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
