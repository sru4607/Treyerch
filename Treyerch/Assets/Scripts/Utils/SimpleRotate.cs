using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    public float degPerSec;
    public bool cw;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cw)
        {
            gameObject.transform.Rotate(0, degPerSec * Time.deltaTime, 0);
        }
        else
        {
            gameObject.transform.Rotate(0, -degPerSec * Time.deltaTime, 0);
        }
    }
}
