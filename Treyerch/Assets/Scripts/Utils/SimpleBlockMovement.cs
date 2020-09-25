using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBlockMovement : MonoBehaviour
{
    public bool vertical;
    public float range;
    private bool pos = true;
    private Vector3 start;

    // Start is called before the first frame update
    void Start()
    {
        start = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (pos)
        {
            if (vertical)
            {
                gameObject.transform.Translate(0, (range / 2.0f) * Time.deltaTime, 0);
                if(start.y + range <= gameObject.transform.position.y)
                {
                    pos = false;
                    Debug.Log("e");
                }
            }
            else
            {
                gameObject.transform.Translate(0, 0, (range / 2.0f) * Time.deltaTime);
                if (start.z + range <= gameObject.transform.position.z)
                {
                    pos = false;
                    Debug.Log("e");
                }
            }
        }

        if (!pos)
        {
            if (vertical)
            {
                gameObject.transform.Translate(0, (-range / 2.0f) * Time.deltaTime, 0);
                if (start.y - range >= gameObject.transform.position.y)
                {
                    pos = true;
                    Debug.Log("e");
                }
            }
            else
            {
                gameObject.transform.Translate(0, 0, (-range / 2.0f) * Time.deltaTime);
                if (start.z - range >= gameObject.transform.position.z)
                {
                    pos = true;
                    Debug.Log("e");
                }
            }
        }
    }
}
