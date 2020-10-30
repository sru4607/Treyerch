using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;
    public float period;
    public float time;

    void FixedUpdate()
    {
        time+=Time.deltaTime;
        float halfPeriod = period/2;
        float currentPeriodTime = time%period;
        Vector3 toMove;
        if(currentPeriodTime < halfPeriod)
        {
            toMove = Vector3.Lerp(start,end,currentPeriodTime%halfPeriod/halfPeriod);
        }
        else{
            toMove = Vector3.Lerp(end,start,currentPeriodTime%halfPeriod/halfPeriod);
        }
        gameObject.GetComponent<Rigidbody>().MovePosition(toMove);
    }
}
