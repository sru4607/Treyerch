#region Packages
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#endregion

public class DestructionMonitor : MonoBehaviour
{
    #region Variables & Inspector Options
    #region Options Tab
    #region Settings
    [Header("Options")]
    [TabGroup("Settings"), PropertyOrder(-1)]
    [PropertyRange(0, 100)]
    public int destructionPercentNeeded = 70;
    [TabGroup("Settings"), PropertyOrder(-1)]
    public float posDeltaMax = 5;
    #endregion

    #region Modes
    [TabGroup("Settings"), PropertyOrder(-1)]
    [Header("Modes")]
    public DetectionMode detectionMode = DetectionMode.Trigger;
    [TabGroup("Settings"), PropertyOrder(-1)]
    [ShowIf("@detectionMode == DetectionMode.Distance")]
    public GameObject trackingObject;
    [TabGroup("Settings"), PropertyOrder(-1)]
    [ShowIf("@detectionMode == DetectionMode.Distance")]
    public float trackingMaxDistance = 10f;

    [TabGroup("Settings"), PropertyOrder(-1)]
    [Header("Events")]
    public UnityEvent OnComplete;
    #endregion
    #endregion

    #region Trackables Tab
    [TabGroup("Trackables")]
    [Button("AutoPopulate", ButtonSizes.Medium), PropertyOrder(-1)]
    public void AutoGetChildRigidBodies()
    {
        Rigidbody[] foundBodies = transform.GetComponentsInChildren<Rigidbody>();

        if (foundBodies.Length > 0)
        {
            trackables = new List<DestructionTrackable>();
            foreach (Rigidbody foundBody in foundBodies)
            {
                trackables.Add(new DestructionTrackable(foundBody.gameObject));
            }
        }
    }

    [TabGroup("Trackables")]
    public List<DestructionTrackable> trackables = new List<DestructionTrackable>();
    #endregion

    #region Debug Tab
    [TabGroup("Debug")]
    [ReadOnly]
    public bool isActive;

    [TabGroup("Debug")]
    [ReadOnly]
    public bool isComplete;
    #endregion

    #region Stored Data
    public enum DetectionMode { Trigger, Distance }

    private float neededDestructionValue;
    private float currentDestructionValue;
    #endregion
    #endregion

    #region Unity Events
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        if(trackables != null && trackables.Count > 0)
        {
            neededDestructionValue = trackables.Count * (destructionPercentNeeded / 100.0f);
        }
    }

    /// <summary>
    /// If the tracked object is close enough, this is active (Distance Mode)
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerStay(Collider other)
    {
        if (detectionMode == DetectionMode.Trigger)
        {
            if (other.gameObject.layer == 9) //Player
            {
                isActive = true;
            }
        }
    }

    /// <summary>
    /// If the tracked object is too far, this is inactive (Distance Mode)
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (detectionMode == DetectionMode.Trigger)
        {
            if (other.gameObject.layer == 9) //Player
            {
                isActive = false;
            }
        }
    }

    /// <summary>
    /// Update the origin position of the tracked objects
    /// </summary>
    public void OnValidate()
    {
        foreach (DestructionTrackable trackable in trackables)
        {
            if (trackable.trackedObject)
            {
                trackable.originalPosition = trackable.trackedObject.transform.position;
            }
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if(detectionMode == DetectionMode.Distance)
        {
            if(trackingObject != null)
            {
                if(Mathf.Abs(Vector3.Distance(trackingObject.transform.position, transform.position)) <= trackingMaxDistance)
                {
                    isActive = true;
                }
                else
                {
                    isActive = false;
                }
            }
        }

        if(isActive && !isComplete)
        {
            currentDestructionValue = 0f;
            foreach (DestructionTrackable trackable in trackables)
            {
                if (trackable.trackedObject && !trackable.reachedMax)
                {
                    trackable.posDelta = Mathf.Abs(Vector3.Distance(trackable.trackedObject.transform.position, trackable.originalPosition));
                    trackable.posDeltaProgress = trackable.posDelta / posDeltaMax;
                    if (trackable.posDeltaProgress >= 1.0f)
                    {
                        trackable.reachedMax = true;
                        currentDestructionValue += 1.0f;
                    }
                    else
                    {
                        currentDestructionValue += trackable.posDeltaProgress;
                    }
                }
                else
                {
                    currentDestructionValue += 1.0f;
                }
            }

            if(currentDestructionValue >= neededDestructionValue)
            {
                isComplete = true;

                if(OnComplete != null)
                {
                    OnComplete.Invoke();
                }
            }
        }
    }
    #endregion

    #region Classes
    [System.Serializable]
    public class DestructionTrackable
    {
        #region Variables & Inspector Options
        [BoxGroup("Trackable", false)]
        public GameObject trackedObject;

        #region Debug
        [BoxGroup("Trackable", false)]
        [FoldoutGroup("Trackable/Debug")]
        [ReadOnly]
        public Vector3 originalPosition;

        [BoxGroup("Trackable", false)]
        [FoldoutGroup("Trackable/Debug")]
        [ReadOnly]
        public float posDelta = 0;

        [BoxGroup("Trackable", false)]
        [FoldoutGroup("Trackable/Debug")]
        [ReadOnly]
        public float posDeltaProgress = 0;

        [BoxGroup("Trackable", false)]
        [FoldoutGroup("Trackable/Debug")]
        [ReadOnly]
        public bool reachedMax = false;
        #endregion
        #endregion

        #region Constructors
        public DestructionTrackable(GameObject objectToTrack)
        {
            trackedObject = objectToTrack;
        }
        #endregion
    }
    #endregion
}