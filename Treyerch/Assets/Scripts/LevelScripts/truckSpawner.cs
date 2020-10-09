﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class truckSpawner : MonoBehaviour
{
    //How min spawn time
    public float minDelay;
    //Delay before first spawn
    private float delay;
    //Max delay time on top of period
    public float maxDelay;
    //How long since last spawn
    private float ActionTime;
    //Prefab to Spawn
    public GameObject truckPrefab;
    //Start Vector
    public Vector3 start;
    //End Vector
    public Vector3 end;
    //TweenLength
    public float length;
    public float startDelay;
    // Start is called before the first frame update
    void Start()
    {
        ActionTime = startDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time>ActionTime){
            Spawn();
            ActionTime=Time.time+Random.Range(minDelay,maxDelay);

        }
    }

    void Spawn(){
        GameObject truck = Instantiate(truckPrefab,start, Quaternion.identity);
        truck.transform.DOMove(end, length).SetEase(Ease.Linear);
        Destroy(truck, length+1);

    }
}
