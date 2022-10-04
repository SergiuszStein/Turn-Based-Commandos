using System.Collections;
using System.Collections.Generic;
using C__Scripts;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _healthPickup;
    [SerializeField] private GameObject _ammoPickup;
    [SerializeField] private GameObject _proxyMine;
    [SerializeField] private GameObject _gasBarrel;
    private int _turnNumber = 1;
    [SerializeField] private PlayerManager _playerManager;
    
    public void SpawnStuff()
    {
        _turnNumber++;

        float _spawnPickup = Random.Range(1f, 10f);

        Vector3 _spawnPosition = new Vector3(
            Random.Range(-25, 25),
            15,
            Random.Range(-25, 25)
        );

        if (_spawnPickup > (3 + (_turnNumber / _playerManager._gameManager._playerNumber)))
        {
            Instantiate(_healthPickup, _spawnPosition, transform.rotation);
        }
        else
        {
            Instantiate(_ammoPickup, _spawnPosition, transform.rotation);
        }

        for (int i = 0; i < ((_turnNumber / _playerManager._gameManager._playerNumber) + 1); i++)
        {
            Vector3 _spawnPosition2 = new Vector3(
                Random.Range(-25, 25),
                15,
                Random.Range(-25, 25)
            );
            
            Instantiate(_proxyMine, _spawnPosition2, transform.rotation);

            Vector3 _spawnPosition3 = new Vector3(
                Random.Range(-25, 25),
                15,
                Random.Range(-25, 25)
            );
                    
            Instantiate(_gasBarrel, _spawnPosition3, transform.rotation);
        }
    }
}
