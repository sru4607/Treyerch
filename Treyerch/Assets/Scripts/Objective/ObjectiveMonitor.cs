using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveMonitor : MonoBehaviour
{
    [TabGroup("Objective Triggers")]
    public List<ObjectiveTrigger> objectiveTriggers = new List<ObjectiveTrigger>();

    [TabGroup("Events")]
    public UnityEvent OnSingleComplete;
    [TabGroup("Events")]
    public UnityEvent OnAllComplete;

    public void UseObjective(int objectiveIndex)
    {
        if(objectiveTriggers != null && objectiveTriggers.Count > 0) //Must be a valid list
        {
            if(objectiveTriggers.Count-1 >= objectiveIndex) //Must be a valid index
            {
                objectiveTriggers[objectiveIndex].totalUses++;

                if(objectiveTriggers[objectiveIndex].singleUseCompletion || objectiveTriggers[objectiveIndex].usesUntilComplete <= objectiveTriggers[objectiveIndex].totalUses)
                {
                    objectiveTriggers[objectiveIndex].isComplete = true;

                    if(objectiveTriggers[objectiveIndex].OnComplete != null)
                    {
                        objectiveTriggers[objectiveIndex].OnComplete.Invoke();
                    }

                    if (DoCompleteCheck())
                    {
                        if (OnAllComplete != null)
                        {
                            OnAllComplete.Invoke();
                        }
                    }
                    else
                    {
                        if (OnSingleComplete != null)
                        {
                            OnSingleComplete.Invoke();
                        }
                    }
                }
                else
                {
                    if (objectiveTriggers[objectiveIndex].OnComplete != null)
                    {
                        objectiveTriggers[objectiveIndex].OnUse.Invoke();
                    }
                }
            }
        }
    }

    private bool DoCompleteCheck()
    {
        if (objectiveTriggers != null && objectiveTriggers.Count > 0) //Must be a valid list
        {
            bool allComplete = true;
            foreach(ObjectiveTrigger objectiveTrigger in objectiveTriggers)
            {
                if (objectiveTrigger.isComplete == false)
                {
                    allComplete = false;
                }
            }

            if(allComplete)
            {
                return true;
            }
        }

        return false;
    }
}

[System.Serializable]
public class ObjectiveTrigger
{
    [BoxGroup("Objective Trigger",false)]
    [Title("$objectiveName")]
    public bool singleUseCompletion = true;

    [BoxGroup("Objective Trigger", false)]
    [HideIf("singleUseCompletion")]
    public int usesUntilComplete = 1;

    [BoxGroup("Objective Trigger", false)]
    [FoldoutGroup("Objective Trigger/Objective Details")]
    public string objectiveName = "New Objective";

    [BoxGroup("Objective Trigger", false)]
    [FoldoutGroup("Objective Trigger/Objective Details")]
    public string objectiveDescription;

    [BoxGroup("Objective Trigger", false)]
    [FoldoutGroup("Objective Trigger/Objective Info")]
    [ReadOnly]
    public int totalUses = 0;

    [BoxGroup("Objective Trigger", false)]
    [FoldoutGroup("Objective Trigger/Objective Info")]
    [ReadOnly]
    public bool isComplete;

    [BoxGroup("Objective Trigger", false)]
    [FoldoutGroup("Objective Trigger/Objective Events")]
    public UnityEvent OnUse;

    [BoxGroup("Objective Trigger", false)]
    [FoldoutGroup("Objective Trigger/Objective Events")]
    public UnityEvent OnComplete;
}

