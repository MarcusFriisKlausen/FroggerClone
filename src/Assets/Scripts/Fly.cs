using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    float moveTime = 1f;
    bool moving = false;
    Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving)
        {
            ChooseTarget();
            StartCoroutine(HandleMove(this.transform.position, this.target));
        }
    }

    void Move(Vector3 start, Vector3 end)
    {
        // Moves from start to end in moveTime seconds (source: https://stackoverflow.com/questions/65814157/unity-move-an-object-to-a-pointtransform-within-a-certain-number-of-seconds)
        StartCoroutine(moveTime.Tweeng( (p)=>transform.position=p,
            start,
            end) 
        );
    }
    
    // Makes it so the fly only chooses a target location when not already flying
    IEnumerator HandleMove(Vector3 start, Vector3 end)
    {
        this.moving = true;
        Move(start, end);
        yield return new WaitForSeconds(moveTime);
        this.moving = false;
    }

    // choose next location to fly to randomly
    void ChooseTarget()
    {
        System.Random rand = new System.Random();

        Vector3 nextTarget = new Vector3((float)rand.Next(-24, 15), 5f, (float)rand.Next(-35, 36));

        this.target = nextTarget;
    }
}
