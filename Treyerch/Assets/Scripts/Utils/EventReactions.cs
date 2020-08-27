#region Packages
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
#endregion

public class EventReactions : MonoBehaviour
{
    #region Variables & Inspector Options
    #region Event Controls
    [TabGroup("Event Controls")]
    [Button(ButtonSizes.Medium), PropertyOrder(-1)]
    public void ResetOptions()
    {
        dockablesOptions = new List<DockableOptions>() { new DockableOptions(DockableOptions.DockableType.Audio), new DockableOptions(DockableOptions.DockableType.Particles), new DockableOptions(DockableOptions.DockableType.Animation), new DockableOptions(DockableOptions.DockableType.Instantiation), new DockableOptions(DockableOptions.DockableType.Events) };
    }

    [TabGroup("Event Controls")]
    [ListDrawerSettings(Expanded = true, IsReadOnly = true), PropertyOrder(0)]
    public List<DockableOptions> dockablesOptions = new List<DockableOptions>() { new DockableOptions(DockableOptions.DockableType.Audio), new DockableOptions(DockableOptions.DockableType.Particles), new DockableOptions(DockableOptions.DockableType.Animation), new DockableOptions(DockableOptions.DockableType.Instantiation), new DockableOptions(DockableOptions.DockableType.Events) };
    #endregion

    #region Dockable Triggers
    [TabGroup("Dockable Triggers")]
    [ListDrawerSettings(Expanded = true, ShowIndexLabels = true, ListElementLabelName = "triggerTitle"), PropertyOrder(1)]
    public List<DockableTrigger> dockableTriggers = new List<DockableTrigger>();
    #endregion

    #region Dockables
    [TabGroup("Dockables")]
    [ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true)]
    public List<EventDockable> eventDockables = new List<EventDockable>();
    #endregion
    #endregion

    #region Methods
    #region Unity Events
    private void OnDrawGizmos()
    {
        if(dockablesOptions != null && dockablesOptions.Count > 0)
        {
            for(int i = 0; i < dockablesOptions.Count; i++)
            {
                if(dockablesOptions[i].readyToAdd == true)
                {
                    dockablesOptions[i].readyToAdd = false;
                    Debug.Log("Adding New Dockable - " + dockablesOptions[i].dockableType.ToString());

                    eventDockables.Add(new EventDockable(dockablesOptions[i].dockableType));
                }
            }
        }

        if (eventDockables != null && eventDockables.Count > 0)
        {
            for (int i = 0; i < eventDockables.Count; i++)
            {
                eventDockables[i].OnValidate();

                if (eventDockables[i].doDestroy == true)
                {
                    eventDockables.Remove(eventDockables[i]);
                }
            }

            if (dockableTriggers != null && dockableTriggers.Count > 0)
            {
                for (int q = 0; q < dockableTriggers.Count; q++)
                {
                    dockableTriggers[q].maxDockables = eventDockables.Count-1;
                    dockableTriggers[q].OnValidate();

                    int dockableEventIndex = dockableTriggers[q].dockableToTrigger;
                    if (eventDockables.Count - 1 >= dockableEventIndex)
                    {
                        EventDockable currentDockable = eventDockables[dockableEventIndex];
                        string newTriggerTitle = "";

                        newTriggerTitle += currentDockable.dockableTitle + " ["+ dockableTriggers[q].triggerGroup+"]";

                        if (dockableTriggers[q].indexToActivate > -1)
                        {
                            if (currentDockable.dockableType == DockableOptions.DockableType.Audio)
                            {
                                if (currentDockable.audioDockable != null && currentDockable.audioDockable.audioList != null)
                                {
                                    List<AudioObject> currentList = currentDockable.audioDockable.audioList;
                                    if (currentList != null && currentList.Count > 0)
                                    {
                                        dockableTriggers[q].maxActivates = currentList.Count - 1;
                                        if (currentList.Count - 1 >= dockableTriggers[q].indexToActivate && currentList[dockableTriggers[q].indexToActivate].audioClip != null)
                                        {
                                            newTriggerTitle += " | " + currentList[dockableTriggers[q].indexToActivate].audioClip.name;
                                        }
                                        else
                                        {
                                            newTriggerTitle += " | " + "Invalid Clip";
                                        }
                                    }
                                }
                            }
                            else if (currentDockable.dockableType == DockableOptions.DockableType.Particles)
                            {
                                if (currentDockable.particleDockable != null && currentDockable.particleDockable.particleList != null)
                                {
                                    List<ParticleObject> currentList = currentDockable.particleDockable.particleList;
                                    if (currentList != null && currentList.Count > 0)
                                    {
                                        dockableTriggers[q].maxActivates = currentList.Count - 1;
                                        if (currentList[dockableTriggers[q].indexToActivate].particleMode == ParticleObject.ParticleMode.InScene && currentList.Count - 1 >= dockableTriggers[q].indexToActivate && currentList[dockableTriggers[q].indexToActivate].particleSystem != null)
                                        {
                                            newTriggerTitle += " | " + currentList[dockableTriggers[q].indexToActivate].particleSystem.gameObject.name;
                                        }
                                        else if (currentList[dockableTriggers[q].indexToActivate].particleMode == ParticleObject.ParticleMode.Instantiated && currentList.Count - 1 >= dockableTriggers[q].indexToActivate && currentList[dockableTriggers[q].indexToActivate].particleToInstantiate != null)
                                        {
                                            newTriggerTitle += " | " + currentList[dockableTriggers[q].indexToActivate].particleToInstantiate.gameObject.name;
                                        }
                                        else
                                        {
                                            newTriggerTitle += " | " + "Invalid Particle System";
                                        }
                                    }
                                }
                            }
                            else if (currentDockable.dockableType == DockableOptions.DockableType.Animation)
                            {
                                if (currentDockable.animationDockable != null && currentDockable.animationDockable.animationList != null)
                                {
                                    List<AnimationObject> currentList = currentDockable.animationDockable.animationList;
                                    if (currentList != null && currentList.Count > 0)
                                    {
                                        dockableTriggers[q].maxActivates = currentList.Count - 1;
                                        if (currentList.Count - 1 >= dockableTriggers[q].indexToActivate && currentList[dockableTriggers[q].indexToActivate].animator != null)
                                        {
                                            newTriggerTitle += " | " + currentList[dockableTriggers[q].indexToActivate].animator.gameObject.name + " (" + currentList[dockableTriggers[q].indexToActivate].animationTriggerType.ToString() + " - " + currentList[dockableTriggers[q].indexToActivate].animationEvent + ")";
                                        }
                                        else
                                        {
                                            newTriggerTitle += " | " + "Invalid Animator";
                                        }
                                    }
                                }
                            }
                            else if (currentDockable.dockableType == DockableOptions.DockableType.Instantiation)
                            {
                                if (currentDockable.instantiationDockable != null && currentDockable.instantiationDockable.instantiationList != null)
                                {
                                    List<InstantiationObject> currentList = currentDockable.instantiationDockable.instantiationList;
                                    if (currentList != null && currentList.Count > 0)
                                    {
                                        dockableTriggers[q].maxActivates = currentList.Count - 1;
                                        if (currentList.Count - 1 >= dockableTriggers[q].indexToActivate && currentList[dockableTriggers[q].indexToActivate].objectToInstantiate != null)
                                        {
                                            newTriggerTitle += " | " + currentList[dockableTriggers[q].indexToActivate].objectToInstantiate.gameObject.name;
                                        }
                                        else
                                        {
                                            newTriggerTitle += " | " + "Invalid Object Prefab";
                                        }
                                    }
                                }
                            }
                            else if (currentDockable.dockableType == DockableOptions.DockableType.Events)
                            {
                                if (currentDockable.eventsDockable != null && currentDockable.eventsDockable.eventsList != null)
                                {
                                    List<EventsObject> currentList = currentDockable.eventsDockable.eventsList;
                                    if (currentList != null && currentList.Count > 0)
                                    {
                                        dockableTriggers[q].maxActivates = currentList.Count - 1;
                                        if (currentList.Count - 1 >= dockableTriggers[q].indexToActivate && currentList[dockableTriggers[q].indexToActivate].unityEvent != null && currentList[dockableTriggers[q].indexToActivate].unityEvent.GetPersistentEventCount() > 0)
                                        {
                                            newTriggerTitle += " | " + currentList[dockableTriggers[q].indexToActivate].unityEvent.GetPersistentMethodName(0);
                                        }
                                        else
                                        {
                                            newTriggerTitle += " | " + "Invalid Unity Event";
                                        }
                                    }
                                }
                            }
                        }
                        else if (dockableTriggers[q].indexToActivate == -1)
                        {
                            newTriggerTitle += " | " + "Increment";
                        }
                        else if (dockableTriggers[q].indexToActivate == -2)
                        {
                            newTriggerTitle += " | " + "Random";
                        }

                        dockableTriggers[q].triggerTitle = newTriggerTitle;
                    }
                    else
                    {
                        if (dockableTriggers[q].triggerTitle != "Invalid Dockable")
                        {
                            dockableTriggers[q].triggerTitle = "Invalid Dockable";
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Activates a dockable trigger of given index
    /// </summary>
    /// <param name="triggerToActive"></param>
    public void ActivteTrigger(int triggerToActive)
    {
        if (eventDockables != null && eventDockables.Count > 0)
        {
            if (dockableTriggers != null && dockableTriggers.Count > 0)
            {
                int dockableEventIndex = dockableTriggers[triggerToActive].dockableToTrigger;

                if (eventDockables.Count - 1 >= dockableEventIndex)
                {
                    eventDockables[dockableEventIndex].activateIndex = dockableTriggers[triggerToActive].indexToActivate;
                    //Debug.Log("[EventReactions] Activating ["+ triggerToActive+ " - " + eventDockables[dockableEventIndex].dockableTitle + "] (" + eventDockables[dockableEventIndex].activateIndex + ")");
                    eventDockables[dockableEventIndex].ActivateDockable();
                }
                else
                {
                    Debug.Log("ERROR | Index to trigger is not within the event dockables list");
                }
            }
            else
            {
                Debug.Log("ERROR | Dockable Triggers is either null or empty");
            }
        }
        else
        {
            Debug.Log("ERROR | Event Dockables is either null or empty");
        }
    }

    /// <summary>
    /// Activates all dockable triggers with a given group index
    /// </summary>
    /// <param name="groupToTrigger"></param>
    public void ActivateTriggerGroup(int groupToTrigger)
    {
        if (eventDockables != null && eventDockables.Count > 0)
        {
            if (dockableTriggers != null && dockableTriggers.Count > 0)
            {
                List<DockableTrigger> groupTriggers = new List<DockableTrigger>();

                for(int i = 0; i < dockableTriggers.Count; i++)
                {
                    if(dockableTriggers[i].triggerGroup == groupToTrigger)
                    {
                        groupTriggers.Add(dockableTriggers[i]);
                    }
                }
                
                if(groupTriggers.Count > 0)
                {
                    foreach(DockableTrigger groupTrigger in groupTriggers)
                    {
                        int dockableEventIndex = groupTrigger.dockableToTrigger;
                        if (eventDockables.Count - 1 >= dockableEventIndex)
                        {
                            eventDockables[dockableEventIndex].activateIndex = groupTrigger.indexToActivate;
                            //Debug.Log("[EventReactions] Activating ["+ triggerToActive+ " - " + eventDockables[dockableEventIndex].dockableTitle + "] (" + eventDockables[dockableEventIndex].activateIndex + ")");
                            eventDockables[dockableEventIndex].ActivateDockable();
                        }
                        else
                        {
                            Debug.Log("ERROR | Index to trigger is not within the event dockables list");
                        }
                    }
                }
            }
            else
            {
                Debug.Log("ERROR | Dockable Triggers is either null or empty");
            }
        }
        else
        {
            Debug.Log("ERROR | Event Dockables is either null or empty");
        }
    }

    /// <summary>
    /// Activates all dockable triggers
    /// </summary>
    public void ActivateAllEventsWithTriggers()
    {
        if (eventDockables != null && eventDockables.Count > 0)
        {
            if (dockableTriggers != null && dockableTriggers.Count > 0)
            {
                for (int i = 0; i < dockableTriggers.Count; i++)
                {
                    int dockableEventIndex = dockableTriggers[i].dockableToTrigger;

                    if (eventDockables.Count - 1 >= dockableEventIndex)
                    {
                        eventDockables[dockableEventIndex].activateIndex = dockableTriggers[i].indexToActivate;
                        eventDockables[dockableEventIndex].ActivateDockable();
                    }
                    else
                    {
                        Debug.Log("ERROR | Index to trigger is not within the event dockables list");
                    }
                }
            }
            else
            {
                Debug.Log("ERROR | Dockable Triggers is either null or empty");
            }
        }
        else
        {
            Debug.Log("ERROR | Event Dockables is either null or empty");
        }
    }

    /// <summary>
    /// Destorys this gameObject or it's parent gameObject
    /// </summary>
    /// <param name="destroyParent"></param>
    public void DestroyObject(bool destroyParent)
    {
        GameObject toDestroy = gameObject;

        if(destroyParent)
        {
            toDestroy = transform.parent.gameObject;
        }

        Destroy(toDestroy);
    }

    /// <summary>
    /// Manually trigger an event with a given index and mode, this does not use triggers
    /// </summary>
    /// <param name="indexToActive"></param>
    /// <param name="modeIndex"></param>
    public void ActivateEventManually(int indexToActive, int modeIndex)
    {
        if(eventDockables != null && eventDockables.Count > 0)
        {
            if(eventDockables.Count-1 >= indexToActive)
            {
                //Triger a specific list index ( = -1 Incremnt | = -2 Random | >= 0 Indexed)
                eventDockables[indexToActive].activateIndex = modeIndex;

                eventDockables[indexToActive].ActivateDockable();
            }
            else
            {
                Debug.Log("ERROR | Index to play is not within the event dockables list");
            }
        }
        else
        {
            Debug.Log("ERROR | Event Dockables is either null or empty");
        }
    }
    #endregion
    #endregion
}

#region Classes
#region Generic Dockable
[System.Serializable]
public class DockableTrigger
{
    #region Variables & Inspector Options
    [TitleGroup("$triggerTitle")]
    [PropertyRange(0, "maxDockables")]
    public int dockableToTrigger;

    [TitleGroup("$triggerTitle")]
    [ShowIf("@activateMode == EventDockable.ActivateMode.Indexed")]
    [PropertyRange(0, "maxActivates")]
    public int indexToActivate;

    [TitleGroup("$triggerTitle")]
    public EventDockable.ActivateMode activateMode;

    [TitleGroup("$triggerTitle")]
    [HorizontalGroup("$triggerTitle/Split"), PropertySpace(SpaceAfter = 4, SpaceBefore = 10)]
    [ReadOnly]
    public int triggerGroup = 0;

    [TitleGroup("$triggerTitle")]
    [HorizontalGroup("$triggerTitle/Split"), PropertySpace(SpaceAfter = 4, SpaceBefore = 10)]
    [Button(ButtonSizes.Small), LabelText("<"), LabelWidth(70)]
    public void DecreaseGroup()
    {
        if (triggerGroup > 0)
        {
            triggerGroup--;
        }
    }

    [TitleGroup("$triggerTitle")]
    [HorizontalGroup("$triggerTitle/Split"), PropertySpace(SpaceAfter = 4, SpaceBefore = 10)]
    [Button(ButtonSizes.Small), LabelText(">"), LabelWidth(70)]
    public void IncreaseGroup()
    {
        triggerGroup++;
    }

    #region Stored Data
    [HideInInspector]
    public string triggerTitle = "New Dockable Trigger";

    [HideInInspector]
    public int maxDockables = 1;

    [HideInInspector]
    public int maxActivates = 1;
    #endregion
    #endregion

    #region Unity Events
    public void OnValidate()
    {
        if(activateMode == EventDockable.ActivateMode.Increment && indexToActivate != -1)
        {
            indexToActivate = -1;
        }
        else if (activateMode == EventDockable.ActivateMode.Random && indexToActivate != -2)
        {
            indexToActivate = -2;
        }
        else if (activateMode == EventDockable.ActivateMode.Indexed && indexToActivate < 0)
        {
            indexToActivate = 0;
        }

        if (indexToActivate == -1 && activateMode != EventDockable.ActivateMode.Increment)
        {
            activateMode = EventDockable.ActivateMode.Increment;
        }
        else if (indexToActivate == -2 && activateMode != EventDockable.ActivateMode.Random)
        {
            activateMode = EventDockable.ActivateMode.Random;
        }

        if(triggerGroup == -1)
        {
            triggerGroup = 0;
        }
    }
    #endregion
}

[System.Serializable]
public class DockableOptions
{
    #region Variables & Inspector Options
    [Button(ButtonSizes.Medium), LabelText("$dockableTitle")]
    public void AddDockable()
    {
        readyToAdd = true;
    }

    #region Stored Data
    public enum DockableType { Audio, Particles, Animation, Instantiation, Events }

    [HideInInspector]
    public bool readyToAdd = false;

    [HideInInspector]
    public DockableType dockableType;

    private string dockableTitle;
    #endregion
    #endregion

    #region Constructors
    public DockableOptions(DockableType type)
    {
        dockableType = type;
        dockableTitle = "Add " + dockableType.ToString();
    }
    #endregion
}

[System.Serializable]
public class EventDockable
{
    #region Variables & Inspector Options
    [Header("Dockable Opstions")]
    [TitleGroup("$dockableTitle", Alignment = TitleAlignments.Centered, BoldTitle = true)]
    public bool showDockableOptions = false;

    [ShowIfGroup("$dockableTitle/showDockableOptions")]
    [BoxGroup("$dockableTitle/showDockableOptions/DockableOptions", ShowLabel = false)]
    [Button(ButtonSizes.Medium)]
    public void DestroyDockable()
    {
        doDestroy = true;
    }

    #region Dockables
    [ShowIf("@dockableType == DockableOptions.DockableType.Audio")]
    [HideLabel]
    public AudioDockable audioDockable;

    [ShowIf("@dockableType == DockableOptions.DockableType.Particles")]
    [HideLabel]
    public ParticleDockable particleDockable;

    [ShowIf("@dockableType == DockableOptions.DockableType.Animation")]
    [HideLabel]
    public AnimationDockable animationDockable;

    [ShowIf("@dockableType == DockableOptions.DockableType.Instantiation")]
    [HideLabel]
    public InstantiationDockable instantiationDockable;

    [ShowIf("@dockableType == DockableOptions.DockableType.Events")]
    [HideLabel]
    public EventsDockable eventsDockable;
    #endregion

    #region Stored Data
    public enum ActivateMode { Indexed, Increment, Random }

    [HideInInspector]
    public bool doDestroy = false;

    [HideInInspector]
    public int activateIndex = -3;

    [HideInInspector]
    public string dockableTitle;

    [HideInInspector]
    public DockableOptions.DockableType dockableType;
    #endregion
    #endregion

    #region Constructors
    public EventDockable(DockableOptions.DockableType type)
    {
        dockableType = type;
        dockableTitle = dockableType.ToString() + " Dockable";
    }
    #endregion

    #region Unity Events
    public void OnValidate()
    {        
        if(dockableType == DockableOptions.DockableType.Audio && audioDockable != null)
        {
            List<AudioObject> currentList = audioDockable.audioList;
            if (currentList != null && currentList.Count > 0)
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    currentList[i].OnValidate();
                    if(currentList[i].testClip == true)
                    {
                        currentList[i].testClip = false;
                        audioDockable.PlayAudio(i);
                    }
                }
            }
        }
        else if (dockableType == DockableOptions.DockableType.Particles && particleDockable != null)
        {
            List<ParticleObject> currentList = particleDockable.particleList;
            if (currentList != null && currentList.Count > 0)
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    currentList[i].OnValidate();
                }
            }
        }
        else if (dockableType == DockableOptions.DockableType.Animation && animationDockable != null)
        {
            List<AnimationObject> currentList = animationDockable.animationList;
            if (currentList != null && currentList.Count > 0)
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    currentList[i].OnValidate();
                }
            }
        }
        else if (dockableType == DockableOptions.DockableType.Events && eventsDockable != null)
        {
            List<EventsObject> currentList = eventsDockable.eventsList;
            if (currentList != null && currentList.Count > 0)
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    currentList[i].OnValidate();
                }
            }
        }
        else if (dockableType == DockableOptions.DockableType.Instantiation && instantiationDockable != null)
        {
            List<InstantiationObject> currentList = instantiationDockable.instantiationList;
            if (currentList != null && currentList.Count > 0)
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    currentList[i].OnValidate();
                }
            }
        }

        if (dockableTitle != dockableType.ToString() + " Dockable")
        {
            dockableTitle = dockableType.ToString() + " Dockable";
        }
    }
    #endregion

    #region Main Methods
    public void ActivateDockable()
    {
        if (activateIndex != -3)
        {
            if (dockableType == DockableOptions.DockableType.Audio)
            {
                audioDockable.PlayAudio(activateIndex);
                activateIndex = -3;
            }
            else if (dockableType == DockableOptions.DockableType.Particles)
            {
                particleDockable.PlayParticle(activateIndex);
                activateIndex = -3;
            }
            else if (dockableType == DockableOptions.DockableType.Animation)
            {
                animationDockable.PlayAnimation(activateIndex);
                activateIndex = -3;
            }
            else if (dockableType == DockableOptions.DockableType.Instantiation)
            {
                instantiationDockable.PlayInstantiation(activateIndex);
                activateIndex = -3;
            }
            else if (dockableType == DockableOptions.DockableType.Events)
            {
                eventsDockable.PlayEvent(activateIndex);
                activateIndex = -3;
            }
        }
        else
        {
            Debug.Log("ERROR | activateIndex has not been set correctly");
        }
    }
    #endregion
}
#endregion

#region Type Dockables
#region Audio
[System.Serializable]
public class AudioDockable
{
    #region Variables & Inspector Options
    [Header("Dockable Properties")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "objectTitle")]
    public List<AudioObject> audioList = new List<AudioObject>();

    #region Stored Data
    [HideInInspector]
    public int currentListIndex = 0;
    #endregion
    #endregion

    #region Dockable Controls
    public void PlayAudio(int indexToPlay)
    {
        if (audioList != null && audioList.Count > 0)
        {
            #region Activate Modes
            if (indexToPlay == -1)
            {
                indexToPlay = currentListIndex;
                currentListIndex++;

                if (currentListIndex >= audioList.Count)
                {
                    currentListIndex = 0;
                }
            } //Increment
            else if (indexToPlay == -2)
            {
                indexToPlay = Random.Range(0, audioList.Count);
            } //Random
            #endregion

            if (audioList.Count - 1 >= indexToPlay)
            {
                if (audioList[indexToPlay].audioSource)
                {
                    audioList[indexToPlay].audioSource.volume = audioList[indexToPlay].volume;
                    audioList[indexToPlay].audioSource.priority = audioList[indexToPlay].priority;
                    audioList[indexToPlay].audioSource.pitch = audioList[indexToPlay].pitch;

                    audioList[indexToPlay].audioSource.PlayOneShot(audioList[indexToPlay].audioClip);
                }
                else
                {
                    Debug.Log("ERROR | No valid audio source found to play from");
                }
            }
            else
            {
                Debug.Log("ERROR | Index to play is not within the audio list");
            }
        }
        else
        {
            Debug.Log("ERROR | Audio List is either null or empty");
        }
    }

    #endregion
}

[System.Serializable]
public class AudioObject
{
    #region Variables & Inspector Options
    [BoxGroup("AudioObject", ShowLabel = false)]
    public AudioSource audioSource;

    [BoxGroup("AudioObject", ShowLabel = false)]
    public AudioClip audioClip;

    [BoxGroup("AudioObject", ShowLabel = false)]
    [FoldoutGroup("AudioObject/Audio Settings")]
    [Range(0,1)]
    public float volume = 1;

    [BoxGroup("AudioObject", ShowLabel = false)]
    [FoldoutGroup("AudioObject/Audio Settings")]
    [Range(0, 256)]
    public int priority = 128;

    [BoxGroup("AudioObject", ShowLabel = false)]
    [FoldoutGroup("AudioObject/Audio Settings")]
    [Range(-3, 3)]
    public float pitch = 1;

    [BoxGroup("AudioObject", ShowLabel = false)]
    [FoldoutGroup("AudioObject/Audio Settings")]
    [Button(ButtonSizes.Small)]
    public void TestClip()
    {
        testClip = true;
    }

    #region Stored Data
    [HideInInspector]
    public string objectTitle = "New Audio Object";

    [HideInInspector]
    public bool testClip = false;
    #endregion
    #endregion

    #region Constructors
    public AudioObject()
    {
        volume = 1;
        priority = 128;
        pitch = 1;
    }
    #endregion

    #region Unity Events
    public void OnValidate()
    {
        if(audioClip != null)
        {
            if(objectTitle != audioClip.name)
            {
                objectTitle = audioClip.name;
            }
        }
    }
    #endregion
}
#endregion

#region Particle
[System.Serializable]
public class ParticleDockable
{
    #region Variables & Inspector Options
    [Header("Dockable Properties")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "objectTitle")]
    public List<ParticleObject> particleList = new List<ParticleObject>();

    #region Stored Data
    [HideInInspector]
    public int currentListIndex = 0;
    #endregion
    #endregion

    #region Dockable Controls
    public void PlayParticle(int indexToPlay)
    {
        if (particleList != null && particleList.Count > 0)
        {
            #region Activate Modes
            if (indexToPlay == -1)
            {
                indexToPlay = currentListIndex;
                currentListIndex++;

                if (currentListIndex >= particleList.Count)
                {
                    currentListIndex = 0;
                }
            } //Increment
            else if (indexToPlay == -2)
            {
                indexToPlay = Random.Range(0, particleList.Count);
            } //Random
            #endregion

            if (particleList.Count - 1 >= indexToPlay)
            {
                if (particleList[indexToPlay].particleMode == ParticleObject.ParticleMode.InScene)
                {
                    if (particleList[indexToPlay].particleSystem)
                    {
                        particleList[indexToPlay].particleSystem.Play();
                    }
                    else
                    {
                        Debug.Log("ERROR | Index to play doesn't contain a particle system");
                    }
                }
                else if(particleList[indexToPlay].particleMode == ParticleObject.ParticleMode.Instantiated)
                {
                    if (particleList[indexToPlay].particleToInstantiate)
                    {
                        if(particleList[indexToPlay].instantiationTransform != null)
                        {
                            GameObject newParticle = GameObject.Instantiate(particleList[indexToPlay].particleToInstantiate);
                            if (particleList[indexToPlay].matchPosition)
                            {
                                newParticle.transform.position = particleList[indexToPlay].instantiationTransform.position;
                            }
                            if (particleList[indexToPlay].matchRotation)
                            {
                                newParticle.transform.rotation = particleList[indexToPlay].instantiationTransform.rotation;
                            }
                            if (particleList[indexToPlay].matchScale)
                            {
                                newParticle.transform.localScale = particleList[indexToPlay].instantiationTransform.localScale;
                            }
                        }
                        else
                        {
                            Debug.Log("ERROR | No valid transform to instantiate at");
                        }
                    }
                    else
                    {
                        Debug.Log("ERROR | Index to play doesn't contain a particle prefab");
                    }
                }
            }
            else
            {
                Debug.Log("ERROR | Index to play is not within the particle list");
            }
        }
        else
        {
            Debug.Log("ERROR | Particle List is either null or empty");
        }
    }
   #endregion
}

[System.Serializable]
public class ParticleObject
{
    #region Variables & Inspector Options
    [BoxGroup("ParticleObject", ShowLabel = false)]
    public ParticleMode particleMode = ParticleMode.InScene;

    [BoxGroup("ParticleObject", ShowLabel = false)]
    [ShowIf("@particleMode == ParticleMode.InScene")]
    public ParticleSystem particleSystem;

    [BoxGroup("ParticleObject", ShowLabel = false)]
    [ShowIf("@particleMode == ParticleMode.Instantiated")]
    public GameObject particleToInstantiate;

    [BoxGroup("ParticleObject", ShowLabel = false)]
    [ShowIf("@particleMode == ParticleMode.Instantiated")]
    public Transform instantiationTransform;

    [BoxGroup("ParticleObject", ShowLabel = false)]
    [ShowIf("@particleMode == ParticleMode.Instantiated && instantiationTransform != null")]
    public bool matchPosition = true;

    [BoxGroup("ParticleObject", ShowLabel = false)]
    [ShowIf("@particleMode == ParticleMode.Instantiated && instantiationTransform != null")]
    public bool matchRotation = true;

    [BoxGroup("ParticleObject", ShowLabel = false)]
    [ShowIf("@particleMode == ParticleMode.Instantiated && instantiationTransform != null")]
    public bool matchScale = false;

    #region Stored Data
    public enum ParticleMode { InScene, Instantiated }

    [HideInInspector]
    public string objectTitle = "New Particle Object";
    #endregion
    #endregion

    #region Unity Events
    public void OnValidate()
    {
        if (particleMode == ParticleMode.InScene)
        {
            if (particleSystem != null)
            {
                if (objectTitle != particleSystem.gameObject.name)
                {
                    objectTitle = particleSystem.gameObject.name;
                }
            }
        }
        else
        {
            if (particleToInstantiate != null)
            {
                if (objectTitle != particleToInstantiate.gameObject.name)
                {
                    objectTitle = particleToInstantiate.gameObject.name;
                }
            }
        }
    }
    #endregion
}
#endregion

#region Animation
[System.Serializable]
public class AnimationDockable
{
    #region Variables & Inspector Options
    [Header("Dockable Properties")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "objectTitle")]
    public List<AnimationObject> animationList = new List<AnimationObject>();

    #region Stored Data
    [HideInInspector]
    public int currentListIndex = 0;
    #endregion
    #endregion

    #region Dockable Controls
    public void PlayAnimation(int indexToPlay)
    {
        if (animationList != null && animationList.Count > 0)
        {
            #region Activate Modes
            if (indexToPlay == -1)
            {
                indexToPlay = currentListIndex;
                currentListIndex++;

                if (currentListIndex >= animationList.Count)
                {
                    currentListIndex = 0;
                }
            } //Increment
            else if (indexToPlay == -2)
            {
                indexToPlay = Random.Range(0, animationList.Count);
            } //Random
            #endregion

            if (animationList.Count - 1 >= indexToPlay)
            {
                AnimationObject currentAnimation = animationList[indexToPlay];
                if (currentAnimation.animator != null)
                {
                    if (currentAnimation.animationTriggerType == AnimationObject.AnimationTriggerType.Bool)
                    {
                        if (currentAnimation.doToggleBool)
                        {
                            currentAnimation.animator.SetBool(currentAnimation.animationEvent, !currentAnimation.animator.GetBool(currentAnimation.animationEvent));
                        }
                        else
                        {
                            currentAnimation.animator.SetBool(currentAnimation.animationEvent, currentAnimation.setBool);
                        }
                    }
                    else if (currentAnimation.animationTriggerType == AnimationObject.AnimationTriggerType.Trigger)
                    {
                        currentAnimation.animator.SetTrigger(currentAnimation.animationEvent);
                    }
                    else if (currentAnimation.animationTriggerType == AnimationObject.AnimationTriggerType.Float)
                    {
                        currentAnimation.animator.SetFloat(currentAnimation.animationEvent, currentAnimation.floatValue);
                    }
                    else if (currentAnimation.animationTriggerType == AnimationObject.AnimationTriggerType.Int)
                    {
                        currentAnimation.animator.SetFloat(currentAnimation.animationEvent, currentAnimation.intValue);
                    }
                }
                else
                {
                    Debug.Log("ERROR | Index to play doesn't contain an animator");
                }
            }
            else
            {
                Debug.Log("ERROR | Index to play is not within the particle list");
            }
        }
        else
        {
            Debug.Log("ERROR | Particle List is either null or empty");
        }
    }
    #endregion
}

[System.Serializable]
public class AnimationObject
{
    #region Variables & Inspector Options
    [BoxGroup("AnimationObject", ShowLabel = false)]
    public Animator animator;

    [BoxGroup("AnimationObject", ShowLabel = false)]
    public string animationEvent = "";

    [BoxGroup("AnimationObject", ShowLabel = false), PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
    public AnimationTriggerType animationTriggerType = AnimationTriggerType.Bool;

    #region Bool
    [BoxGroup("AnimationObject", ShowLabel = false)]
    [ShowIf("@animationTriggerType == AnimationTriggerType.Bool")]
    public bool doToggleBool;

    [BoxGroup("AnimationObject", ShowLabel = false)]
    [ShowIf("@animationTriggerType == AnimationTriggerType.Bool && doToggleBool == false")]
    public bool setBool;
    #endregion

    #region Float
    [BoxGroup("AnimationObject", ShowLabel = false)]
    [ShowIf("@animationTriggerType == AnimationTriggerType.Float")]
    public float floatValue;
    #endregion

    #region Int
    [BoxGroup("AnimationObject", ShowLabel = false)]
    [ShowIf("@animationTriggerType == AnimationTriggerType.Int")]
    public int intValue;
    #endregion

    #region Stored Data
    public enum AnimationTriggerType { Bool, Trigger, Float, Int}

    [HideInInspector]
    public string objectTitle = "New Animation Object";
    #endregion
    #endregion

    #region Unity Events
    public void OnValidate()
    {
        if (animator != null)
        {
            string titleString = animator.gameObject.name + " (" + animationTriggerType.ToString() + " - " + animationEvent + ")";
            if (objectTitle != titleString)
            {
                objectTitle = titleString;
            }
        }
    }
    #endregion
}
#endregion

#region Events
[System.Serializable]
public class EventsDockable
{
    #region Variables & Inspector Options
    [Header("Dockable Properties")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "objectTitle")]
    public List<EventsObject> eventsList = new List<EventsObject>();

    #region Stored Data
    [HideInInspector]
    public int currentListIndex = 0;
    #endregion
    #endregion

    #region Dockable Controls
    public void PlayEvent(int indexToPlay)
    {
        if (eventsList != null && eventsList.Count > 0)
        {
            #region Activate Modes
            if (indexToPlay == -1)
            {
                indexToPlay = currentListIndex;
                currentListIndex++;

                if (currentListIndex >= eventsList.Count)
                {
                    currentListIndex = 0;
                }
            } //Increment
            else if (indexToPlay == -2)
            {
                indexToPlay = Random.Range(0, eventsList.Count);
            } //Random
            #endregion

            if (eventsList.Count - 1 >= indexToPlay)
            {
                if (eventsList[indexToPlay].unityEvent != null && eventsList[indexToPlay].unityEvent.GetPersistentEventCount() > 0)
                {
                    eventsList[indexToPlay].unityEvent.Invoke();
                }
                else
                {
                    Debug.Log("ERROR | Index to play doesn't contain a unity event");
                }
            }
            else
            {
                Debug.Log("ERROR | Index to play is not within the events list");
            }
        }
        else
        {
            Debug.Log("ERROR | Events List is either null or empty");
        }
    }
    #endregion
}

[System.Serializable]
public class EventsObject
{
    #region Variables & Inspector Options
    [BoxGroup("EventsObject", ShowLabel = false)]
    public UnityEvent unityEvent;

    #region Stored Data
    [HideInInspector]
    public string objectTitle = "New Event Object";
    #endregion
    #endregion

    #region Unity Events
    public void OnValidate()
    {
        if (unityEvent != null && unityEvent.GetPersistentEventCount() > 0)
        {
            if (objectTitle != unityEvent.GetPersistentMethodName(0))
            {
                objectTitle = unityEvent.GetPersistentMethodName(0);
            }
        }
    }
    #endregion
}
#endregion

#region Instantiation
[System.Serializable]
public class InstantiationDockable
{
    #region Variables & Inspector Options
    [Header("Dockable Properties")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "objectTitle")]
    public List<InstantiationObject> instantiationList = new List<InstantiationObject>();

    #region Stored Data
    [HideInInspector]
    public int currentListIndex = 0;
    #endregion
    #endregion

    #region Dockable Controls
    public void PlayInstantiation(int indexToPlay)
    {
        if (instantiationList != null && instantiationList.Count > 0)
        {
            #region Activate Modes
            if (indexToPlay == -1)
            {
                indexToPlay = currentListIndex;
                currentListIndex++;

                if (currentListIndex >= instantiationList.Count)
                {
                    currentListIndex = 0;
                }
            } //Increment
            else if (indexToPlay == -2)
            {
                indexToPlay = Random.Range(0, instantiationList.Count);
            } //Random
            #endregion

            if (instantiationList.Count - 1 >= indexToPlay)
            {
                if (instantiationList[indexToPlay].objectToInstantiate)
                {
                    if (instantiationList[indexToPlay].instantiationTransform != null)
                    {
                        Transform tomatch = instantiationList[indexToPlay].instantiationTransform;

                        if (instantiationList[indexToPlay].differentMatchTransform)
                        {
                            if (instantiationList[indexToPlay].matchTransform != null)
                            {
                                tomatch = instantiationList[indexToPlay].matchTransform;
                            }
                            else
                            {
                                Debug.Log("ERROR | Match Transform is null");
                            }
                        }

                        GameObject newObject = GameObject.Instantiate(instantiationList[indexToPlay].objectToInstantiate, tomatch.position, tomatch.rotation);

                        if (instantiationList[indexToPlay].matchScale)
                        {
                            newObject.transform.localScale = tomatch.localScale;
                        }

                        if(instantiationList[indexToPlay].initialVelocity != Vector3.zero)
                        {
                            Rigidbody foundBody = newObject.GetComponent<Rigidbody>();
                            if(foundBody)
                            {
                                foundBody.AddForce(instantiationList[indexToPlay].instantiationTransform.forward * (instantiationList[indexToPlay].initialVelocity).magnitude);
                            }
                            else
                            {
                                Debug.Log("ERROR | No Rigidbody found while trying to use velocity");
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("ERROR | Index to play doesn't contain an object prefab");
                }
            }
            else
            {
                Debug.Log("ERROR | Index to play is not within the instantiation list");
            }
        }
        else
        {
            Debug.Log("ERROR | Instantiation List is either null or empty");
        }
    }
    #endregion
}

[System.Serializable]
public class InstantiationObject
{
    #region Variables & Inspector Options
    [BoxGroup("InstantiationObject", ShowLabel = false)]
    public GameObject objectToInstantiate;

    [BoxGroup("InstantiationObject", ShowLabel = false)]
    public Transform instantiationTransform;

    [BoxGroup("InstantiationObject", ShowLabel = false)]
    public bool differentMatchTransform = false;

    [BoxGroup("InstantiationObject", ShowLabel = false)]
    [ShowIf("differentMatchTransform"), PropertySpace(SpaceBefore = 10, SpaceAfter = 0)]
    public Transform matchTransform;

    [BoxGroup("InstantiationObject", ShowLabel = false)]
    [ShowIf("@instantiationTransform != null")]
    public bool matchPosition = true;

    [BoxGroup("InstantiationObject", ShowLabel = false)]
    [ShowIf("@instantiationTransform != null")]
    public bool matchRotation = true;

    [BoxGroup("InstantiationObject", ShowLabel = false)]
    [ShowIf("@instantiationTransform != null")]
    public bool matchScale = false;

    [BoxGroup("InstantiationObject", ShowLabel = false)]
    [ShowIf("@instantiationTransform != null")]
    public Vector3 initialVelocity = new Vector3();

    #region Stored Data
    [HideInInspector]
    public string objectTitle = "New Instantiation Object";
    #endregion
    #endregion

    #region Unity Events
    public void OnValidate()
    {
        if (objectToInstantiate != null)
        {
            if (objectTitle != objectToInstantiate.gameObject.name)
            {
                objectTitle = objectToInstantiate.gameObject.name;
            }
        }
    }
    #endregion
}
#endregion
#endregion
#endregion