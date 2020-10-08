using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay(Collision collision){
        if(collision.gameObject.tag == "Player"){
            Debug.Log("Enter");
            collision.gameObject.transform.parent = gameObject.transform;
        }
    }

    void OnCollisionExit(Collision collision){
        if(collision.gameObject.tag == "Player"){
            Debug.Log("Left");
            collision.gameObject.transform.parent = null;
        }

    }
}
