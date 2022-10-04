using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasBarrel : MonoBehaviour
{
    [SerializeField] private GameObject _boomModel;
    [SerializeField] private float _boomRadius;
    [SerializeField] private int _barrelDamage;
    
    public void Damage()
    {
        Collider[] _hitColliders = Physics.OverlapSphere(transform.position, _boomRadius);

        for (int i = 0; i < _hitColliders.Length; i++)
        {
            GameObject _hit = _hitColliders[i].gameObject;
            
            if (_hit.layer == LayerMask.NameToLayer("Player"))
            {
                _hit.GetComponent<Player>().Damage(_barrelDamage);
            }
            else if (_hit.layer == LayerMask.NameToLayer("GasBarrel"))
            {
                if (_hit != this.gameObject)
                {
                    _hit.GetComponent<GasBarrel>().DamageFromBarrel(this.gameObject);
                }
            }
        }

        Instantiate(_boomModel, transform.position, Quaternion.identity);
        
        Destroy(this.gameObject);
    }

    public void DamageFromBarrel(GameObject barrel)
    {
        Collider[] _hitColliders = Physics.OverlapSphere(transform.position, _boomRadius);

        for (int i = 0; i < _hitColliders.Length; i++)
        {
            GameObject _hit = _hitColliders[i].gameObject;
            
            if (_hit.layer == LayerMask.NameToLayer("Player"))
            {
                _hit.GetComponent<Player>().Damage(_barrelDamage);
            }
            else if (_hit.layer == LayerMask.NameToLayer("GasBarrel"))
            {
                if (_hit != this.gameObject)
                {
                    if (_hit != barrel)
                    {
                        _hit.GetComponent<GasBarrel>().DamageFromBarrel(this.gameObject);
                    }
                }
            }
        }

        Instantiate(_boomModel, transform.position, Quaternion.identity);
        
        Destroy(this.gameObject);
    }
}
