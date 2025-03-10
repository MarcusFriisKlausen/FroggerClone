using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FrogAI : MonoBehaviour
{
    public Frog frog; // Reference to the Frog script
    private Direction nextDirection = Direction.None;

    void Start()
    {
        if (frog == null)
        {
            frog = GetComponent<Frog>();
        }
    }

    void Update()
    {
        if (!frog)
        {
            Debug.LogError("Frog script is not assigned.");
            return;
        }

        // Check preconditions and decide the next action
        MakeDecision();

        // Move in the decided direction
        if (nextDirection != Direction.None)
        {
            frog.MoveDirection(nextDirection);
            nextDirection = Direction.None;
        }
    }

    void MakeDecision()
    {
        // Priority-based decision-making based on the HTN

        // 1. Check if there's a fly nearby to eat
        if (frog.flyInRange && frog.fliesEaten < 2)
        {
            EatFly();
            return;
        }

        // 2. Digest a fly to enable jumping if a car is approaching
        if (frog.fliesEaten > 0 && IsCarApproachingLaneClose())
        {
            DigestFly();
            return;
        }

        // 3. Move toward the goal or fly
        if (MoveTowardTarget())
        {
            return;
        }
            

        // 4. Wait if unable to move or take any other action
        Wait();
    }

    void EatFly()
    {
        // Trigger the fly-eating behavior
        frog.EatFly();
        Debug.Log("AI: Eating fly.");
    }

    void DigestFly()
    {
        // Trigger the fly-digesting behavior
        frog.DigestFly();
        Debug.Log("AI: Digesting fly for a jump.");
    }

    bool MoveTowardTarget()
    {
        // Determine if the AI can move toward a target (goal or fly)

        // Example target: the goal position (top of the screen)
        Vector3 target = new Vector3(-28.5f, frog.transform.position.y, frog.transform.position.z);

        // Find the best direction to move toward the target
        Direction bestDirection = GetBestDirection(target);
        if (bestDirection != Direction.None)
        {
            nextDirection = bestDirection;
            return true;
        }

        return false;
    }

    void Wait()
    {
        Debug.Log("AI: Waiting as no valid actions are available.");
    }

    bool IsCarApproachingLaneClose()
    {
        float[] lanesX = GameObject.Find("Cars").GetComponent<Cars>().lanesX;
        int[] laneSpeeds = GameObject.Find("Cars").GetComponent<Cars>().laneSpeeds;
        List<Vector3> vehiclePositions = frog.VehiclePositions();
        Vector3 frogPos = frog.transform.position;

        foreach (Vector3 vehiclePos in vehiclePositions)
        {
            float xDiff = RoundToNearestHalf(vehiclePos).x - RoundToNearestHalf(frogPos).x;
            if (xDiff == 0f)
            {
                bool carDirectionTowardsFrog;
                int lane = Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x);
                bool carCloseZ = Mathf.Abs(vehiclePos.z - frogPos.z) / laneSpeeds[lane] < 0.3f;
                if (lane % 2 == 0)
                {
                    if ((vehiclePos.z - 4.5f) < frogPos.z)
                    {
                        carDirectionTowardsFrog = true;
                    }
                    else
                    {
                        carDirectionTowardsFrog = false;
                    }
                }
                else
                {
                    if ((vehiclePos.z + 4.5f) > frogPos.z)
                    {
                        carDirectionTowardsFrog = true;
                    }
                    else
                    {
                        carDirectionTowardsFrog = false;
                    }
                }
                if (carDirectionTowardsFrog && carCloseZ)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool IsCarApproachingLane()
    {
        List<Vector3> vehiclePositions = frog.VehiclePositions();
        Vector3 frogPos = frog.transform.position;

        foreach (Vector3 vehiclePos in vehiclePositions)
        {
            if (RoundToNearestHalf(vehiclePos).x - RoundToNearestHalf(frogPos).x == 0f &&
                Mathf.Abs(vehiclePos.z - frogPos.z) < 30f)
            {
                return true;
            }
        }

        return false;
    }

    bool IsCarApproachingAbove()
    {
        List<Vector3> vehiclePositions = frog.VehiclePositions();
        int[] laneSpeeds = GameObject.Find("Cars").GetComponent<Cars>().laneSpeeds;
        Vector3 frogPos = frog.transform.position;
        float[] lanesX = GameObject.Find("Cars").GetComponent<Cars>().lanesX;

        foreach (Vector3 vehiclePos in vehiclePositions)
        {
            bool carAbove = RoundToNearestHalf(vehiclePos).x < RoundToNearestHalf(frogPos).x;
            bool carDirectlyAbove = Mathf.Abs(vehiclePos.x - frogPos.x) < 6;
            bool carCloseZ;
            bool carDirectionTowardsFrog;
            int lane = -1;
            // frog is on pavement
            if (Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x - 4.5f) > -1)
            {
                lane = Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x - 4.5f);
            }
            // frog is on road in front of road
            else if (Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x - 5f) > -1)
            {
                lane = Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x - 5f);
            }
            // frog is on raod in front of pavement
            else
            {
                return false;
            }
            carCloseZ = Mathf.Abs(vehiclePos.z - frogPos.z) / laneSpeeds[lane] < 1f;
            if (lane % 2 == 0)
            {
                if ((vehiclePos.z - 4.5f) < frogPos.z)
                {
                    carDirectionTowardsFrog = true;
                }
                else
                {
                    carDirectionTowardsFrog = false;
                }
            }
            else
            {
                if ((vehiclePos.z + 4.5f) > frogPos.z)
                {
                    carDirectionTowardsFrog = true;
                }
                else
                {
                    carDirectionTowardsFrog = false;
                }
            }



            if (carAbove &&
                carDirectlyAbove &&
                carCloseZ &&
                carDirectionTowardsFrog)
            {
                return true;
            }
        }

        return false;
    }

    bool IsCarApproachingBelow()
    {
        // Example logic for detecting approaching cars
        List<Vector3> vehiclePositions = frog.VehiclePositions();
        int[] laneSpeeds = GameObject.Find("Cars").GetComponent<Cars>().laneSpeeds;
        Vector3 frogPos = frog.transform.position;
        float[] lanesX = GameObject.Find("Cars").GetComponent<Cars>().lanesX;

        foreach (Vector3 vehiclePos in vehiclePositions)
        {
            bool carBelow = RoundToNearestHalf(vehiclePos).x > RoundToNearestHalf(frogPos).x;
            bool carDirectlyBelow = Mathf.Abs(vehiclePos.x - frogPos.x) < 6;
            bool carCloseZ;
            bool carDirectionTowardsFrog;
            int lane = -1;
            if (Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x + 4.5f) > -1)
            {
                lane = Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x + 4.5f);
            }
            else if (Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x + 5f) > -1)
            {
                lane = Array.IndexOf(lanesX, RoundToNearestHalf(frogPos).x + 5f);
            }
            else
            {
                return false;
            }
            carCloseZ = Mathf.Abs(vehiclePos.z - frogPos.z) / laneSpeeds[lane] < 1f;
            if (lane % 2 == 0)
            {
                if ((vehiclePos.z - 4.5f) < frogPos.z)
                {
                    carDirectionTowardsFrog = true;
                }
                else
                {
                    carDirectionTowardsFrog = false;
                }
            }
            else
            {
                if ((vehiclePos.z + 4.5f) > frogPos.z)
                {
                    carDirectionTowardsFrog = true;
                }
                else
                {
                    carDirectionTowardsFrog = false;
                }
            }

            if (carBelow &&
                carDirectlyBelow &&
                carCloseZ &&
                carDirectionTowardsFrog)
            {
                return true;
            }
        }

        return false;
    }

    Direction GetBestDirection(Vector3 target)
    {
        // Compute the best direction to move toward the target
        Vector3 frogPos = frog.transform.position;

        // Manhattan distance heuristic
        float xDiff = target.x - frogPos.x;
        float zDiff = target.z - frogPos.z;

        if (!IsCarApproachingAbove()) {
            Debug.Log("Moving Up");
            return Direction.Up;
        }
        else if (!IsCarApproachingLane())
        {
            Debug.Log("Staying, no car close in lane");
            return Direction.None;
        }
        else if (!IsCarApproachingBelow())
        {
            Debug.Log("Moving Down");
            return Direction.Down;
        }
        else
        {
            Debug.Log("Moving sideways, cars from above, below and in lane");
            float frogX = RoundToNearestHalf(frog.transform.position).x;
            float[] lanesX = GameObject.Find("Cars").GetComponent<Cars>().lanesX;
            if (Array.IndexOf(lanesX, frogX) % 2 == 0)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        }
    }

    // Rounding function
    Vector3 RoundToNearestHalf(Vector3 position)
    {
        return new Vector3(
            (float)Math.Round(position.x * 2, MidpointRounding.AwayFromZero) / 2,
            (float)Math.Round(position.y * 2, MidpointRounding.AwayFromZero) / 2,
            (float)Math.Round(position.z * 2, MidpointRounding.AwayFromZero) / 2
        );
    }
}
