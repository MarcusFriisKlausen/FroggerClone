using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

// enum for direction of next movement
public enum Direction
{
    None,
    Up,
    Down,
    Right,
    Left
}

public class Frog : MonoBehaviour
{
    public bool flyInRange = false;
    public int fliesEaten = 0;
    public float moveTime = .33f;
    public float jumpTime = .5f;
    // Set for if movement is from pavement to lane and vice versa
    HashSet<(float, Direction)> toOrFromPavementSet = new HashSet<(float, Direction)>();
    Direction nextDir;
    public bool moving = false;
    public bool jumping = false;
    // Start is called before the first frame update
    void Start()
    {
        float paveStart = this.transform.position.x;
        float paveMiddle = paveStart - 2 * 4.5f - 3 * 5f;
        float paveEnd = paveStart - 4 * 4.5f - 6 * 5f;
        
        // pavement x positions
        HashSet<float> pavementX = new HashSet<float>(){paveStart, paveMiddle, paveEnd};

        // setting up toOrFromPavementSet
        foreach (float x in pavementX)
        {
            // moving up to pavement
            toOrFromPavementSet.Add((x + 4.5f, Direction.Up)); //
            // moving up from pavement
            toOrFromPavementSet.Add((x, Direction.Up));
            // moving down from pavement
            toOrFromPavementSet.Add((x, Direction.Down));
            // moving down to pavement
            toOrFromPavementSet.Add((x - 4.5f, Direction.Down));
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckFrogWin();
        FlyInRange();
        TimeLoss();
        if (Input.GetKey(KeyCode.F))
        {
            EatFly();
        }
        if (Input.GetKey(KeyCode.E))
        {
            DigestFly();
        }
        FindDirection();
        if (this.nextDir != Direction.None)
        {
            MoveDirection(nextDir);
            this.nextDir = Direction.None;
        }
    }

    void FindDirection()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.nextDir = Direction.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.nextDir = Direction.Down;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.nextDir = Direction.Right;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.nextDir = Direction.Left;
        }
    }

    void Move(Vector3 start, Vector3 end, float t)
    {
        // Moves from start to end in moveTime seconds (source: https://stackoverflow.com/questions/65814157/unity-move-an-object-to-a-pointtransform-within-a-certain-number-of-seconds)
        StartCoroutine(t.Tweeng( (p)=>transform.position=p,
            start,
            end) 
        );
    }
    
    // Makes it so the player can't buffer inputs
    IEnumerator HandleMove(Vector3 start, Vector3 end, float t)
    {
        this.moving = true;
        Move(start, end, t);
        yield return new WaitForSeconds(t);
        this.moving = false;
        if (this.jumping)
        {
            yield return new WaitForSeconds(this.jumpTime - t);
            this.jumping = false;
        }
    }

    float CalculateMoveDistance(Vector3 start, Direction dir)
    {
        float distance;
        bool toOrFromPavement;
        
        // sideways movement distance is constant
        if (dir == Direction.Right || dir == Direction.Left)
        {
            switch(dir)
            {
                case Direction.Left:
                    distance = -2.5f;
                    break;
                case Direction.Right:
                    distance = 2.5f;
                    break;
                default:
                    throw new ArgumentException("Invalid Direction");
            }
            return distance;
        }
        
        // Round position to handle floating point shenanigans
        float xPos = (float)Math.Round(start.x * 2, MidpointRounding.AwayFromZero) / 2;
        if (toOrFromPavementSet.Contains((xPos, dir)))
        {
            toOrFromPavement = true;
        }
        else
        {
            toOrFromPavement = false;
        }

        switch((toOrFromPavement, dir))
        {
            case (true, Direction.Up):
                distance = -4.5f;
                break;
            case (false, Direction.Up):
                distance = -5f;
                break;
            case (true, Direction.Down):
                distance = 4.5f;
                break;
            case (false, Direction.Down):
                distance = 5f;
                break;
            default:
                throw new ArgumentException("Invalid Direction");
        }
        return distance;
    }

    // Check if position is within the bounds of the map
    bool CanMoveTo(Vector3 position)
    {
        bool canMoveTo;

        // bounds
        float leftZ = -42.5f;
        float rightZ = 42.5f;
        float botX = 19.5f;
        float topX = -28.5f;
        
        // Round positions to handle floating point shenanigans
        float xPos = (float)Math.Round(position.x * 2, MidpointRounding.AwayFromZero) / 2;
        float zPos = (float)Math.Round(position.z * 2, MidpointRounding.AwayFromZero) / 2;

        if (zPos >= leftZ && zPos <= rightZ && xPos >= topX && xPos <= botX)
        {
            canMoveTo = true;
        }
        else
        {
            canMoveTo = false;
        }
        return canMoveTo;
    }

    public void MoveDirection(Direction dir)
    {
        // Don't move if already moving
        if (this.moving || this.jumping)
        {
            return;
        }

        Vector3 start = this.transform.position;
        Vector3 end;
        float distance = CalculateMoveDistance(start, dir);
        switch(dir)
        {
            case Direction.Up:
                end = new Vector3(start.x + distance, start.y, start.z);
                break;
            case Direction.Down:
                end = new Vector3(start.x + distance, start.y, start.z);
                break;
            case Direction.Right:
                end = new Vector3(start.x, start.y, start.z + distance);
                break;
            case Direction.Left:
                end = new Vector3(start.x, start.y, start.z + distance);
                break;
            default:
                throw new ArgumentException("Invalid Direction");
        }
        if (CanMoveTo(end))
        {
            StartCoroutine(HandleMove(start, end, this.moveTime));
        }
    }

    // Collision handling
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log("Collided with a Car");
            // Restart game
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Frog looses given time
    void TimeLoss()
    {
        if (Time.timeSinceLevelLoad > 60f)
        {
            Debug.Log("Lost to time");
            // Restart game
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
    }

    // Distance to fly
    float FlyDistance()
    {
        GameObject fly = GameObject.FindWithTag("Fly");
        float dist = Vector3.Distance(this.transform.position, fly.transform.position);
        return dist;
    }

    void FlyInRange()
    {
        if (FlyDistance() < 10f)
        {
            this.flyInRange = true;
        }
        else
        {
            this.flyInRange = false;
        }
    }

    // TODO: maybe make a field for bool onCooldown?
    void DestroyFly()
    {
        GameObject fly = GameObject.FindWithTag("Fly");
        Destroy(fly);
    }

    // Eat fly if in range by pressing the f-key
    public void EatFly()
    {
        if (FlyDistance() < 10f && fliesEaten < 2)
        {
            DestroyFly();
            fliesEaten++;
        }
    }

    public void DigestFly()
    {
        if (fliesEaten > 0)
        {
            if (!this.jumping)
            {
                this.jumping = true;
                Jump();
                fliesEaten--;
            }
        }
    }

    void Jump()
    {
        if (!this.moving)
        {
            Vector3 start = this.transform.position;
            Vector3 end = new Vector3(start.x, start.y + 10f, start.z);
            StartCoroutine(HandleMove(start, end, jumpTime / 2));
            StartCoroutine(WaitTillNoVehicle(start, end));
        }
    }

    // Coroutine for waiting till vehicle under frog has passed
    IEnumerator WaitTillNoVehicle(Vector3 start, Vector3 end)
    {
        bool isVehicle = true;

        // Get all vehicle positions
        List<Vector3> vehiclePositions = VehiclePositions();
        while (isVehicle)
        {
            if (!this.moving)
            {
                isVehicle = false;
            }
            foreach (Vector3 vehiclePosition in vehiclePositions)
            {
                bool xBool = RoundToNearestHalf(vehiclePosition).x == RoundToNearestHalf(start).x;
                bool yBool = RoundToNearestHalf(vehiclePosition).y == 0f;
                bool zBool = Mathf.Abs(RoundToNearestHalf(vehiclePosition).z - RoundToNearestHalf(start).z) <= 4; // Check if in range of vehicle of length 8
                if (xBool && yBool && zBool)
                {
                    isVehicle = true;
                    break;
                }
            }
            yield return null;
        }
        StartCoroutine(HandleMove(this.transform.position, start, jumpTime / 2));
    }
    
    public List<Vector3> VehiclePositions()
    {
        GameObject carContainer = GameObject.Find("Cars");
        List<Vector3> positions = new List<Vector3>();
        foreach (Transform child in carContainer.transform)
        {
            foreach (Transform grandChild in child)
            {
                positions.Add(grandChild.position);
            }
        }
        return positions;
    }

    // Frog to check if frog has won
    void CheckFrogWin()
    {
        if (RoundToNearestHalf(this.transform.position).x == -28.5f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
