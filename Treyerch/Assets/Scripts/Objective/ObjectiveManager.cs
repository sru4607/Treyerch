using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManagerEventCompletion : UnityEvent<Objective>{}
public class ManagerEventActivated : UnityEvent<Objective>{}

public class ObjectiveManager : MonoBehaviour
{
    //stored Instance
    public static ObjectiveManager instance = null;

#region members   
    //List of all objectives with scripts activated at runtime
    public List<Objective> m_AllObjectives;
    //Events
    public ManagerEventCompletion m_EventCompleted; 
    public ManagerEventActivated m_EventActivated; 
    public GameObject m_TextObject;
    public GameObject m_ObjectiveContent;
#endregion

    //Get the instance of the manager
    public static ObjectiveManager GetManager(){
        return instance;
    }
    void Awake(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Debug.Log("Multiple Instances of Objective Manager Instantiated. Destroying newest instance");
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start(){
        //Get All Objectives
        Objective[] createdObjectives = FindObjectsOfType(typeof(Objective)) as Objective[];
        foreach(Objective currentObjective in createdObjectives)
        {
            m_AllObjectives.Add(currentObjective);
        }
        m_AllObjectives.Sort(SortBySortValues);
        foreach(Objective toAdd in m_AllObjectives)
        {
            GameObject created = Instantiate(m_TextObject, m_ObjectiveContent.transform);
            created.GetComponent<Text>().text = toAdd.m_ObjectiveData.objectiveTitle;
        }
        if (m_EventCompleted == null)
            m_EventCompleted = new ManagerEventCompletion();
        m_EventCompleted.AddListener(EventCompleted);

        //To-Do (Populate Menu with all objectives)
    }
    int SortBySortValues(Objective a, Objective b){
        if(a.m_ObjectiveData.sortVaule > b.m_ObjectiveData.sortVaule){
            return 1;
        } 
        else if(a.m_ObjectiveData.sortVaule == b.m_ObjectiveData.sortVaule){
            return 0;
        }
        else{
            return -1;
        }
    }
#region EventTriggers
    //To run on every event completion
    void EventCompleted(Objective completedObjective){
        if(!completedObjective.m_Activated){
            return;
        }
        //If there are prerequistes to completing this task
        if(completedObjective.m_prerequisites.Length != 0){
            //for every prerequisite check if completed. If a objective is not completed stop checking
            foreach(Objective parentObjective in completedObjective.m_prerequisites){
                if(!parentObjective.m_Completed){
                    Debug.Log("Parent Objectives not completed");
                    return;
                }
            }
        }
        Debug.Log("Event Attempt Completed:" + completedObjective.m_ObjectiveData.objectiveTitle);
        completedObjective.ObjectiveCompletedEvent.Invoke();
    }
    //To-Do (Activate Quests)
    public void ActivateQuest(Objective toActivate){
        toActivate.ObjectiveActivatedEvent.Invoke();
    }
}
#endregion