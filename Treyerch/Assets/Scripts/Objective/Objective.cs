using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectiveCompletion : UnityEvent{}
public class ObjectiveActivation : UnityEvent{}
public class Objective : MonoBehaviour
{
    public ObjectiveScriptableObject m_ObjectiveData;
    public ObjectiveCompletion ObjectiveCompletedEvent;
    public ObjectiveActivation ObjectiveActivatedEvent;
    public bool m_Completed;
    public bool m_Displayed;
    public bool m_Activated;
    public Objective[] m_prerequisites;
    public List<Objective> children;
    public GameObject m_TextElement;
    //Links events and initializes them
    void Start(){
        children = new List<Objective>();
        if (ObjectiveCompletedEvent == null)
            ObjectiveCompletedEvent = new ObjectiveCompletion();
        ObjectiveCompletedEvent.AddListener(EventCompleted);
        if (ObjectiveActivatedEvent == null)
            ObjectiveActivatedEvent = new ObjectiveActivation();
        ObjectiveActivatedEvent.AddListener(EventActivated);
    }
    //Hides objective and sets completion
    void EventCompleted(){
        m_Completed = true;
        m_Displayed = false;
        m_TextElement.GetComponent<Text>().color = Color.green;
    }
    //Triggers upon activation
    void EventActivated(){
        m_Activated = true;
    }

    public void PropigateParents(){
        foreach(Objective parent in m_prerequisites){
            parent.children.Add(this);
        }
    }

}
