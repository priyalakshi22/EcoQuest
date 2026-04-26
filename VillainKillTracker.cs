using UnityEngine;

public class VillainKillTracker : MonoBehaviour
{
    public GameObject cubeToShow; 
    public int killThreshold = 5;
    private int currentKills = 0;

    public void VillainKilled()
    {
        currentKills++;
        Debug.Log("Villains killed: " + currentKills);

        if (currentKills >= killThreshold && cubeToShow != null)
        {
            cubeToShow.SetActive(true); 
        }
    }
}
