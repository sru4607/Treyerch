using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinSpawner : MonoBehaviour
{
    public GameObject penguin;
    private int countNum = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        countNum++;
        if (countNum == 5)
        {
            GameObject newPenguin = Instantiate(penguin, gameObject.transform.position, gameObject.transform.rotation);
            newPenguin.AddComponent<KillPenguin>();
            countNum = 0;
        }
    }
}
