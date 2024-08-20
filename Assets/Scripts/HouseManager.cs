using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour
{
    public GameObject[] housePrefab; // The house prefab to spawn
    public int numberOfHouses = 10; // Number of houses to spawn
    public float houseSize = 1f; // Approximate size of each house

    public List<Vector3> spawnedPositions = new List<Vector3>(); // List to track spawned positions
    private Camera mainCamera;
    public int i_HouseSpawned;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void SpawnHouse()
    {
        int maxAttempts = 100; // Maximum attempts to find a valid position
        int attempts = 0;
        Vector3 spawnPosition = Vector3.zero;

        bool validPositionFound = false;
        while (attempts < maxAttempts && !validPositionFound)
        {
            // Generate a random position within the screen bounds
            spawnPosition = GetRandomPositionWithinScreen();

            // Check if this position overlaps with any existing house
            validPositionFound = true;
            foreach (Vector3 pos in spawnedPositions)
            {
                if (Vector3.Distance(spawnPosition, pos) < houseSize)
                {
                    validPositionFound = false;
                    break;
                }
            }

            attempts++;
        }

        // If a valid position is found, spawn the house and add the position to the list
        if (validPositionFound)
        {
            GameObject gb = housePrefab[Random.Range(0,housePrefab.Length)];
            Instantiate(gb, spawnPosition, Quaternion.identity);
            i_HouseSpawned++;
            spawnedPositions.Add(spawnPosition);
        }
        else
        {
            Debug.LogWarning("Could not find a valid position for a new house after several attempts.");
        }
    }

    Vector3 GetRandomPositionWithinScreen()
    {
        // Get screen bounds in world coordinates
        Vector3 minScreenBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 maxScreenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        // Generate a random position within the screen bounds
        float randomX = Random.Range(minScreenBounds.x + houseSize, maxScreenBounds.x - houseSize);
        float randomY = Random.Range(minScreenBounds.y + houseSize, maxScreenBounds.y - houseSize);

        return new Vector3(randomX, randomY, 0f);
    }
}
