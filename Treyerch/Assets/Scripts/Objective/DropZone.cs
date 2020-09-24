#region Packages
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;
#endregion

public class DropZone : MonoBehaviour
{
    #region Variables & Inspector Options
    [TabGroup("Setup")]
    [Header("Setup Options")]
    #region Setup Options
    [Tooltip("Select if the player only needs to bring one of the possible objects within the list to this dropzone to trigger")]
    public bool allowSingleObjectFromList = false;

    [TabGroup("Setup")]
    [SerializeField, HideIf("allowSingleObjectFromList")]
    private bool orderMatters = false;

    [TabGroup("Setup")]
    [HideIf("allowSingleObjectFromList"), ReadOnly, ShowIf("orderMatters"), PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
    public int currentOrder = 0;

    [TabGroup("Setup")]
    [Tooltip("Should the object disappear when touching the drop zone")]
    public bool destroyDroppedObject = true;

    [TabGroup("Setup")]
    [HideIf("destroyDroppedObject")]
    public bool LerpToBottom = false;

    [TabGroup("Setup")]
    [ShowIf("LerpToBottom"), PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
    public float IdealBottom;

    [TabGroup("Setup")]
    public bool destroyInvalidObjects = false;

    [TabGroup("Setup")]
    [Tooltip("Select if the dropzone should continue to react forever without being used up")]
    public bool MultipleUses = true;

    [TabGroup("Setup")]
    [HideIf("MultipleUses"), PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
    public bool DestroyAfterFinish = true;

    [TabGroup("Setup"), PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
    public bool verboseLogging = false;

    [TabGroup("Setup")]
    [Header("Setup References")]
    public MeshRenderer meshRenderer;
    [TabGroup("Setup")]
    public DropZoneObjectSet dropZoneObjectSet;
    #endregion

    [Space(10)]
    [TabGroup("Colors")]
    [Header("Color Settings")]
    #region Color Settings
    public bool useColor = true;
    [TabGroup("Colors")]
    public string shaderColor;

    [TabGroup("Colors")]
    [Tooltip("Color that will be displayed when no objects are interacting")]
    [ShowIf("useColor")]
    public float validColorGracePeriod = 3;

    [TabGroup("Colors")]
    [Tooltip("Color that will be displayed when no objects are interacting")]
    [ShowIf("useColor")]
    public Color defaultColor;

    [TabGroup("Colors")]
    [Tooltip("Color that will be displayed when a correct object has interacted")]
    [ShowIf("useColor")]
    public Color validColor;

    [TabGroup("Colors"), PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
    [Tooltip("Color that will be displayed when an incorrect object has interacted")]
    [ShowIf("useColor")]
    public Color invalidColor;



    [TabGroup("Events")]
    [Title("Trigger Parent Events")]
    [SerializeField]
    protected UnityEvent OnActivated = new UnityEvent();

    [TabGroup("Events")]
    [SerializeField]
    protected UnityEvent OnDisabled = new UnityEvent();

    [TabGroup("Events"), PropertyOrder(1)]
    [Title("Generic Events")]
    public UnityEvent onWrongObjectDropped = new UnityEvent();

    [TabGroup("Events"), PropertyOrder(1)]
    public IntEvent onCorrectObjectDropped = new IntEvent();

    [TabGroup("Events"), PropertyOrder(1), PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
    public UnityEvent onDroppedZoneSolved = new UnityEvent();
    #endregion

    [TabGroup("Compatibles")]
    [Header("Compatible Objects")]
    #region Compatible Objects
    [ListDrawerSettings(Expanded = true), PropertySpace(SpaceAfter = 10, SpaceBefore = 0)]
    public List<ObjectCount> objectList;
    #endregion

    #region Debug
    [TabGroup("Debug")]
    [ReadOnly]
    public float progress;
    [TabGroup("Debug")]
    [ReadOnly]
    public GameObject spawnedObject;
    [TabGroup("Debug")]
    [ReadOnly]
    public List<GameObject> invalidObjects = new List<GameObject>();
    [TabGroup("Debug")]
    [ReadOnly]
    public List<GameObject> validObjects = new List<GameObject>();
    #endregion

    #region Stored Data
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    private Dictionary<string, StorageCount> storage = new Dictionary<string, StorageCount>();
    private bool activated;
    public bool Activated { get { return activated; } }
    public float Progress { get { return progress; } }
    private Material mat;
    private bool invalid, triggered;
    private bool isActiveGracePeriod = false;

    #endregion
    #endregion

    #region Methods
    #region Unity Events
    // Use this for initialization
    private void Start()
    {
        if (useColor)
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            mat = meshRenderer.material;
        }

        foreach (ObjectCount o in objectList)
        {
            storage.Add(o.objectID, new StorageCount(o.count));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnedObject == null || (spawnedObject != null && spawnedObject != other.gameObject))
        {
            if (verboseLogging)
                Debug.Log("Trigger entered");

            RemoveDestroyedObjects();

            DropZoneObject d = other.GetComponent<DropZoneObject>();
            if (d == null || !d.ready)
            {
                return;
            }

            ProcessObject(d);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (useColor && !isActiveGracePeriod)
        {
            if (invalidObjects.Contains(other.gameObject))
            {
                if (mat.color != invalidColor)
                {
                    SetMaterialColor(invalidColor);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!destroyInvalidObjects && !destroyDroppedObject)
        {
            RemoveDestroyedObjects();
            invalidObjects.Remove(other.gameObject);

            if (invalidObjects.Count == 0)
            {
                if (useColor)
                    SetMaterialColor(defaultColor);
            }
        }
        else
        {
            if (invalidObjects.Count > 0)
            {
                if (invalidObjects.Contains(other.gameObject))
                {
                    invalidObjects.Remove(other.gameObject);

                    if (invalidObjects.Count == 0)
                    {
                        if (useColor)
                            SetMaterialColor(defaultColor);
                    }
                }
            }
        }
    }
    #endregion

    #region Main Methods
    private void ProcessObject(DropZoneObject d)
    {
        if (AddObject(d))
        {
            if (useColor)
            {
                SetMaterialColor(validColor);
                isActiveGracePeriod = true;
                Invoke("ResetGracePeriod", validColorGracePeriod);
            }
        }
        else
        {
            if (!destroyInvalidObjects)
                invalidObjects.Add(d.transform.parent.gameObject);
            else
                DestroyObject(d.transform.parent.gameObject); // Don't have to destroy it over the network because this is all getting called by everyone

            if (useColor && !isActiveGracePeriod)
                SetMaterialColor(invalidColor);
        }
    }

    /// <summary>
    /// Attempt to add an object to storage.
    /// </summary>
    /// <param name="other">Object to add</param>
    /// <returns>true if addition was successful, false otherwise.</returns>
    private bool AddObject(DropZoneObject d)
    {
        if (!storage.ContainsKey(d.ObjectID) || storage[d.ObjectID].Full)
        {
            onWrongObjectDropped.Invoke();
            return false;
        }

        if (!allowSingleObjectFromList && orderMatters)
        {
            if (objectList[currentOrder].objectID == d.ObjectID)
                currentOrder++;
            else
            {
                onWrongObjectDropped.Invoke();
                return false;
            }
        }


        // increment the object's counter
        storage[d.ObjectID].Add();
        // recalculate progress to completion
        progress = (float)storage.Values.Sum((x) => x.Count) / storage.Values.Sum((x) => x.Capacity);


        // activate if full
        if (!allowSingleObjectFromList)
        {
            if (!destroyDroppedObject) //Adds the gameobject to a list of valid objects and makes it not grabbable
            {
                validObjects.Add(d.gameObject);

                if (LerpToBottom)
                    d.transform.DOLocalMoveY(IdealBottom, .5f);
            }

            if (activated = storage.Values.All((x) => x.Full) && !triggered) //if last item to complete, complete
            {
                onDroppedZoneSolved.Invoke();
                ActivateDropZone();
                OnActivated.Invoke();
            }
            else // correct but not complete
            {
                for (int i = 0; i < objectList.Count; i++)
                {
                    if (objectList[i].objectID == d.ObjectID)
                    {
                        onCorrectObjectDropped.Invoke(i);
                        break;
                    }
                }
            }
        }
        else
        {
            if (storage.Values.Count > 0 && !triggered) //Complete
            {
                if (destroyDroppedObject)
                    DestroyObject(d.transform.gameObject); //Does not call over network because its called for everyone separately
                else
                {
                    validObjects.Add(d.gameObject);
                }

                for (int i = 0; i < objectList.Count; i++)
                {
                    if (objectList[i].objectID == d.ObjectID)
                    {
                        onCorrectObjectDropped.Invoke(i);
                        break;
                    }
                }

                onDroppedZoneSolved.Invoke();
                ActivateDropZone();
                OnActivated.Invoke();
            }
        }
        return true;
    }

    /// <summary>
    /// Event trigger that handles all the trigger properties before the RPC
    /// </summary>
    public void ActivateDropZone()
    {
        triggered = true;
        SetMaterialColor(validColor);

        if (MultipleUses)
            ResetDropZone();
        else
        {
            if (dropZoneObjectSet != null)
            {
                dropZoneObjectSet.SetGameObject(validObjects[0].gameObject);
                meshRenderer.enabled = false;
                this.enabled = false;
            }
            else
            {
                if (DestroyAfterFinish) //Person who finished up handles clearing everything
                {
                    //Debug.Log("Destroying");
                    for (int i = 0; i < validObjects.Count(); i++)
                    {
                        if (validObjects[i] != null)
                        {
                            DestroyObject(validObjects[i]);
                            //Debug.Log("Destroying Object");
                        }
                    }
                    validObjects.Clear();
                }
            }
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Resets the drop zone back to its OnStart state
    /// </summary>
    public void ResetDropZone()
    {
        foreach (GameObject go in validObjects) //Clears valid objects
        {
            if (go != null)
                DestroyObject(go);
        }

        foreach (GameObject go in invalidObjects)
        {
            if (go != null)
                DestroyObject(go);
        }

        currentOrder = 0;
        validObjects.Clear();
        invalidObjects.Clear();
        storage.Clear();
        progress = 0;
        foreach (ObjectCount o in objectList)
        {
            storage.Add(o.objectID, new StorageCount(o.count));
        }
        activated = false;
        triggered = false;
    }

    /// <summary>
    /// Remove objects that are destroyed before exiting trigger
    /// </summary>
    private void RemoveDestroyedObjects()
    {
        for (int i = 0; i < invalidObjects.Count(); i++)
        {
            if (invalidObjects[i] == null)
                invalidObjects.Remove(invalidObjects[i]);
        }
    }


    /// <summary>
    /// Destroys object over network
    /// </summary>
    void DestroyObject(GameObject d)
    {
        Destroy(d);
    }

    /// <summary>
    /// Sets the material Color
    /// </summary>
    /// <param name="color">Color to use.</param>
    private void SetMaterialColor(Color color)
    {
        if (useColor)
            mat.SetColor(shaderColor, color);
            //mat.color = color;
    }

    private void ResetGracePeriod()
    {
        isActiveGracePeriod = false;
        if (invalidObjects.Count == 0)
        {
            SetMaterialColor(defaultColor);
        }
    }

    /// <summary>
    /// Sets the material scale
    /// </summary>
    /// <param name="scale">value to use.</param>
    private void SetScale(float scale)
    {
        mat.SetFloat("_Scale", scale);
    }
    #endregion
    #endregion

    #region Classes & Structs
    /// <summary>
    /// Class used within the disctionary of stored objects to compare IDs with
    /// </summary>
    private class StorageCount
    {
        public int Capacity { get; private set; }
        public int Count { get; private set; }
        public bool Full { get { return Count >= Capacity; } }

        public StorageCount(int capacity)
        {
            Capacity = capacity;
        }

        public void Add()
        {
            ++Count;
        }
    }

    /// <summary>
    /// Stored Object with an ID and the total number needed to trigger this drop zone
    /// </summary>
    [System.Serializable]
    public class ObjectCount
    {
        [Header("Interactable Object")]
        [Tooltip("ID string that will represent an interactable object found on the drop zone object script")]
        public string objectID;
        [Tooltip("The total number of this object that must interact with the drop zone to trigger")]
        public int count = 1;
    }

    [System.Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }
    #endregion
}