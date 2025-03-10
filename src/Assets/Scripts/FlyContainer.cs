using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyContainer : MonoBehaviour
{
    public GameObject flyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        SpawnFly();
    }

    // Update is called once per frame
    void Update()
    {
        RespawnFly();
    }

    void RespawnFly()
    {
        if (this.transform.childCount == 0)
        {
            SpawnFly();
        }
    }

    void SpawnFly()
    {
        System.Random rand = new System.Random();
        
        GameObject car = Instantiate(
            flyPrefab, 
            new Vector3((float)rand.Next(-29, 20), 5f, (float)rand.Next(-42, 43)),
            Quaternion.identity,
            GameObject.Find("FlyContainer").transform
        );
    }
}
