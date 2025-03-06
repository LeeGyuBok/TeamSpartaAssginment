using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSpawn : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    private const float SpawnPointDistanceInterval = 0.2f;

    private const float SpawnInterval = 1.4f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnZombie), 1f, SpawnInterval);
    }

    private void SpawnZombie()
    {
        int randomValue = Random.Range(-1, 2);
        Vector2 spawnPoint = new Vector2(transform.position.x + randomValue * SpawnPointDistanceInterval, transform.position.y + randomValue * SpawnPointDistanceInterval);
        GameObject zombie = Instantiate(zombiePrefab, spawnPoint, Quaternion.identity);
        
        switch (randomValue)
        {
            case -1:
                zombie.layer = LayerMask.NameToLayer("Zombie_Downward");
                break;
            case 0:
                zombie.layer = LayerMask.NameToLayer("Zombie_Center");
                break;
            case 1:
                zombie.layer = LayerMask.NameToLayer("Zombie_Upward");
                break;
        }
    }
}
        
