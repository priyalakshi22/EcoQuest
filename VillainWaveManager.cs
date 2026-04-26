using UnityEngine;

public class VillainWaveManager : MonoBehaviour
{
    public VillainSpawner[] spawners;
    private int currentSpawnerIndex = 0;

    void Start()
    {
        if (spawners.Length > 0)
        {
            spawners[0].StartSpawning(this); 
        }
    }

    public void NotifySpawnerFinished()
    {
        currentSpawnerIndex++;
        if (currentSpawnerIndex < spawners.Length)
        {
            spawners[currentSpawnerIndex].StartSpawning(this);
        }
        else
        {
            Debug.Log("All spawners finished!");
        }
    }
}
