using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private Coin _coinPrefab;

    private void Start()
    {
        SpawnCoins();
    }

    private void SpawnCoins()
    {
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            CreateCoin(_spawnPoints[i]);
        }
    }

    private void CreateCoin(Transform _point)
    {
        PhotonNetwork.Instantiate(_coinPrefab.name, _point.position, _coinPrefab.transform.rotation);
    }
}
