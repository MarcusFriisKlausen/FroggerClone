using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cars : MonoBehaviour
{
    public int[] laneSpeeds; 
    // x position for each lane
    public float[] lanesX = new float[8];
    public GameObject carPrefab;
    public int nCars = 12;
    public float x = .75f;
    // Start is called before the first frame update
    void Start()
    {
        // setup lanesX
        for (int i = 0; i < 4; i++)
        {
            lanesX[i] = 15f - (5f * i);
        }
        for (int i = 0; i < 4; i++)
        {
            lanesX[i + 4] = -9f - (5f * i);
        }
        SpawnCars(nCars);
    }

    void SpawnCars(int n)
    {
        // Assign speed of each lane
        AssignLaneSpeeds();

        // Track occupied positions for each lane
        Dictionary<int, List<float>> occupiedRanges = new Dictionary<int, List<float>>();

        System.Random rand = new System.Random();

        // Spawn one car per lane in a random position
        for (int i = 0; i < 8; i++)
        {
            int dir = (i % 2 == 0) ? -1 : 1;

            // Randomize position for the car in this lane
            float randomZ;
            int attempts = 100; // Limit attempts to find a valid position
            do
            {
                randomZ = rand.Next(1, 40) * dir;
                attempts--;
            }
            while (attempts > 0 && IsRangeOccupied(occupiedRanges.ContainsKey(i) ? occupiedRanges[i] : new List<float>(), randomZ, 8));

            if (attempts == 0)
            {
                Debug.LogWarning("No valid positions available for the initial 8 cars. Terminating SpawnCars.");
                return;
            }

            // Name of the lane
            string lane = "Lane" + (i + 1).ToString();

            // Spawn car
            GameObject car = Instantiate(
                carPrefab, 
                new Vector3(lanesX[i], 0f, randomZ),
                Quaternion.identity,
                GameObject.Find(lane).transform
            );
            car.transform.Rotate(new Vector3(0f, 90f, 0f));
            car.GetComponent<Car>().velocity = this.laneSpeeds[i] * this.x;

            // Mark the range as occupied
            if (!occupiedRanges.ContainsKey(i))
                occupiedRanges[i] = new List<float>();
            MarkRangeAsOccupied(occupiedRanges[i], randomZ, 8);
        }

        // Spawn n - 8 additional cars
        if (n > 8)
        {
            int additionalCars = n - 8;

            for (int count = 0; count < additionalCars; count++)
            {
                int attempts = 100; // Limit attempts to find a valid position
                int randomLane;
                float randomZ;

                do
                {
                    randomLane = rand.Next(0, 8);
                    float randomDir = (randomLane % 2 == 0) ? -1f : 1f;
                    randomZ = rand.Next(1, 40) * randomDir;
                    attempts--;
                }
                while (attempts > 0 && IsRangeOccupied(occupiedRanges[randomLane], randomZ, 8));

                if (attempts == 0)
                {
                    Debug.LogWarning("No valid positions available for additional cars. Terminating SpawnCars.");
                    return;
                }

                // Name of the lane
                string lane = "Lane" + (randomLane + 1).ToString();

                // Spawn car
                GameObject car = Instantiate(
                    carPrefab, 
                    new Vector3(lanesX[randomLane], 0f, randomZ),
                    Quaternion.identity,
                    GameObject.Find(lane).transform
                );
                car.transform.Rotate(new Vector3(0f, 90f, 0f));
                car.GetComponent<Car>().velocity = this.laneSpeeds[randomLane] * this.x;

                // Mark the range as occupied
                MarkRangeAsOccupied(occupiedRanges[randomLane], randomZ, 8);
            }
        }
    }

    // Helper method to check if a range is occupied
    bool IsRangeOccupied(List<float> occupiedRanges, float position, float length)
    {
        foreach (float rangeStart in occupiedRanges)
        {
            if (Mathf.Abs(rangeStart - position) < length)
                return true;
        }
        return false;
    }

    // Helper method to mark a range as occupied
    void MarkRangeAsOccupied(List<float> occupiedRanges, float position, float length)
    {
        occupiedRanges.Add(position);
    }

    void AssignLaneSpeeds()
    {
        // array for speed of the lanes
        int[] lanes = new int[8];
        System.Random rand = new System.Random();

        for (int i = 0; i < lanes.Length; i++)
        {
            lanes[i] = rand.Next(10, 101); // speed 10 equates to 10 seconds traversal while speed 100 equates to 1 second traversal
        }

        this.laneSpeeds = lanes;
    }

}
