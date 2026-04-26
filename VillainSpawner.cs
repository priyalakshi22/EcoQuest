using UnityEngine;
using System.Collections.Generic;

public class VillainSpawner : MonoBehaviour
{
    public GameObject villainPrefab;
    public int totalToSpawn = 5;
    public float spawnInterval = 1f;
    public Transform player;

    private int spawnedCount = 0;
    private float nextSpawnTime = 0f;
    private bool isSpawning = false;
    private List<GameObject> spawnedVillains = new List<GameObject>();
    private VillainWaveManager waveManager;

    public void StartSpawning(VillainWaveManager manager)
    {
        waveManager = manager;
        isSpawning = true;
        spawnedCount = 0;
        spawnedVillains.Clear();
        nextSpawnTime = Time.time;
    }

    void Update()
    {
        if (!isSpawning || spawnedCount >= totalToSpawn) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnVillain();
            nextSpawnTime = Time.time + spawnInterval;
        }

        // Check if all villains are dead
        if (spawnedCount == totalToSpawn)
        {
            bool allDead = true;
            foreach (GameObject villain in spawnedVillains)
            {
                if (villain != null)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
            {
                isSpawning = false;
                Debug.Log(name + " finished its wave.");
                waveManager.NotifySpawnerFinished();
            }
        }
    }

    public void SpawnVillain()
    {
        GameObject newVillain = Instantiate(villainPrefab, transform.position, transform.rotation);

        VillainAI ai = newVillain.GetComponent<VillainAI>();
        if (ai != null && player != null)
        {
            ai.player = player;
        }

        spawnedVillains.Add(newVillain);
        spawnedCount++;
    }
}
