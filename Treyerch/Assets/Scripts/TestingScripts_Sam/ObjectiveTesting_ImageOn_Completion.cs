using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTesting_ImageOn_Completion : MonoBehaviour
{
    public KeyCode toActivate;
    public Color toSet;
    public Objective toCheck;
    // Start is called before the first frame update
    void Start()
    {
        toCheck.ObjectiveCompletedEvent.AddListener(OnEventComplete);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(toActivate)){
            ObjectiveManager.GetManager().m_EventCompleted.Invoke(toCheck);
        }
    }

    void OnEventComplete(){
        gameObject.GetComponent<Image>().color = toSet;
    }
}
