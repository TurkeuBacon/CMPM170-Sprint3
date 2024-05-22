using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnRate;
    public GameObject enemyPrefab;
    private float lastSpawnTime;

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastSpawnTime >= spawnRate) {
            lastSpawnTime = Time.time;
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
