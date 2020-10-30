using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class truckScript : MonoBehaviour
{
    public float endZ;
    public Vector3 velocity;
    public bool reversed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //gameObject.GetComponent<Rigidbody>().velocity = velocity;
        Vector3 scaledVelocity = velocity * Time.deltaTime;
        Vector3 newPos = gameObject.transform.position + scaledVelocity;
        gameObject.GetComponent<Rigidbody>().MovePosition(newPos);
        if(!reversed && gameObject.transform.position.z > endZ){
            Destroy(gameObject);
        }
        else if(reversed && gameObject.transform.position.z < endZ){
            Destroy(gameObject);
        }
    }
}
