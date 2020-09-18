using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_Target : MonoBehaviour
{
    public GameObject target;
    public Objective objective;

    private void OnTriggerStay(Collider other){
        //If the object in the bounds is the target and the objective is not completed - complete the objective
        if(other.gameObject == target && !objective.m_Completed){
            ObjectiveManager.GetManager().m_EventCompleted.Invoke(objective);
        }
    }
}
