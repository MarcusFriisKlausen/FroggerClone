using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public GameObject carPrefab;
    string laneName;
    public int lane;
    float[] lanesX = new float[8];
    public int moveDirection;
    public Rigidbody rb;
    public float velocity = 10f;
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

        this.moveDirection = 1;
        laneName = this.transform.parent.gameObject.name;
        char laneChar = laneName[laneName.Length - 1];
        lane = laneChar - '0';
        if (lane % 2 == 0)
        {
            this.moveDirection = -1;
            if (this.velocity > 0)
            {
                this.velocity = this.velocity * moveDirection;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        OutOfBounds();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (new Vector3(0f, 0f, 1f) * velocity * Time.fixedDeltaTime));
    }

    // Destroy car if out of bounds
    void OutOfBounds()
    {
        if (this.transform.position.z > 50f || this.transform.position.z < -50f)
        {
            GameObject car = Instantiate(
                carPrefab, 
                new Vector3(lanesX[lane - 1], 0f, 49f * (-moveDirection)),
                Quaternion.identity,
                GameObject.Find(laneName).transform
            );
            car.transform.Rotate(new Vector3(0f, 90f, 0f));

            Destroy(this.gameObject);
        }
    }
}
