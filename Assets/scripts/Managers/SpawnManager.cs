using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("The 5 Locations")]
    public Transform[] spawnPoints; // Drag your 5 SpawnPoint objects here

    [Header("The Power-Ups")]
    public GameObject commonPowerUp; // Green (Most Common)
    public GameObject rarePowerUp;   // Blue (Rare)
    public GameObject epicPowerUp;   // Red (Very Rare)

    void Start()
    {
        SpawnAll();
    }

    void SpawnAll()
    {
        // Loop through every spawn point in our list
        foreach (Transform location in spawnPoints)
        {
            // 1. Roll the Dice (0 to 100)
            int dice = Random.Range(0, 100);
            GameObject selectedPrefab;

            // 2. Decide which item to pick based on Rarity
            if (dice < 60)
            {
                // 0 to 59 (60% Chance) -> Common
                selectedPrefab = commonPowerUp;
            }
            else if (dice < 90)
            {
                // 60 to 89 (30% Chance) -> Rare
                selectedPrefab = rarePowerUp;
            }
            else
            {
                // 90 to 99 (10% Chance) -> Epic
                selectedPrefab = epicPowerUp;
            }

            // 3. Spawn the winner at the specific location
            Instantiate(selectedPrefab, location.position, Quaternion.identity);
        }
    }
}