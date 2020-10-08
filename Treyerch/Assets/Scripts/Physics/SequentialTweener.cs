#region Packages
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
#endregion

public class SequentialTweener : MonoBehaviour
{
    #region Variables & Inspector Options
    #region Tracker
    [BoxGroup("Tracker", CenterLabel = true)]
    [ShowIf("@tracker == null"), PropertyOrder(-1)]
    [Button("Add Tracker", ButtonSizes.Medium)]
    public void AddTracker()
    {
        if (tracker == null)
        {
#if UNITY_EDITOR
            ActiveEditorTracker.sharedTracker.isLocked = true;
#endif
            tracker = new GameObject();
            tracker.transform.parent = transform.parent;
            tracker.name = gameObject.name + " - Tracker ["+ trackerOverrideIndex+"]";
            OnTrackerIndexChange();
        }
    }

    [BoxGroup("Tracker", CenterLabel = true)]
    [ShowIf("@tracker != null"), PropertyOrder(-1)]
    [Button("Remove Tracker", ButtonSizes.Medium)]
    public void RemoveTracker()
    {
        if (tracker != null)
        {
            DestroyImmediate(tracker);
            tracker = null;

#if UNITY_EDITOR
            ActiveEditorTracker.sharedTracker.isLocked = false;
#endif
        }
    }

    [BoxGroup("Tracker", CenterLabel = true)]
    [ShowIf("@tracker != null"), PropertyRange(0, "trackerMax"), PropertyOrder(0)]
    [OnValueChanged("OnTrackerIndexChange")]
    public int trackerOverrideIndex;
    #endregion

    #region Sequences Tab
    [TabGroup("Sequences")]
    [ListDrawerSettings(ListElementLabelName = "displayTitle")]
    public List<SequenceTween> sequences = new List<SequenceTween>();
    #endregion

    #region Settings Tab
    [TabGroup("Settings")]
    public bool enableOnStart;
    [TabGroup("Settings")]
    public bool doLoop = true;
    [TabGroup("Settings")]
    public bool doAutoAdvance = true;
    [TabGroup("Settings")]
    public bool isInfinite = true;
    [TabGroup("Settings")]
    [HideIf("isInfinite")]
    public int sequenceLimit = 0;
    #endregion

    #region Visualize Tab
    [TabGroup("Visualize")]
    public MeshFilter meshFilter;
    [TabGroup("Visualize")]
    [ShowIf("meshFilter")]
    public bool visualizeDestinations;
    #endregion

    #region Stored Data
    [HideInInspector]
    public int sequenceIndex = 0;

    [HideInInspector]
    public bool isActive;

    private int timesLeftToPlay = 0;
    private Sequence moveTweener;
    private Sequence rotateTweener;
    private Sequence scaleTweener;
    private GameObject tracker;
    private int trackerMax = 0;
    private int toTrack = 0;
    private bool readyToTrack = false;
    #endregion
    #endregion

    #region Methods
    #region Unity Events
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        RemoveTracker();

        timesLeftToPlay = sequenceLimit;

        if (sequences != null && sequences.Count > 0)
        {
            if (enableOnStart)
            {
                PlaySequenceFromIndex(0);
            }
        }
    }

    /// <summary>
    /// Handles the positioning of the trackers and destination gizmos
    /// </summary>
    private void OnDrawGizmos()
    {
        if (meshFilter && visualizeDestinations)
        {
            if (sequences != null && sequences.Count > 0)
            {
                Vector3 visualizePos = transform.position;
                Quaternion visualizeRot = transform.rotation;
                Vector3 visualizeScale = transform.localScale;

                Vector3 currentPos = transform.position;
                Quaternion currentRot = transform.rotation;
                Vector3 currentScale = transform.localScale;

                for (int i = 0; i < sequences.Count; i++)
                {
                    if (sequences[i].tweenType == SequenceTween.TweenType.Transform)
                    {
                        currentPos = visualizePos;
                        currentRot = visualizeRot;
                        currentScale = visualizeScale;

                        Gizmos.color = sequences[i].visualizeColor;
                        if (sequences[i].doMove)
                        {
                            currentPos = sequences[i].moveTo;
                            visualizePos = sequences[i].moveTo;
                        }
                        if (sequences[i].doRotate)
                        {
                            currentRot = sequences[i].rotateTo;
                            visualizeRot = sequences[i].rotateTo;
                        }
                        if (sequences[i].doScale)
                        {
                            currentScale = sequences[i].scaleTo;
                            visualizeScale = sequences[i].scaleTo;
                        }

                        Gizmos.DrawWireMesh(meshFilter.sharedMesh, currentPos, currentRot, currentScale);
                    }
                }
            }
        }

        if(sequences != null)
        {
            if (sequences.Count > 0)
            {
                if(trackerMax < sequences.Count - 1 || trackerMax > sequences.Count - 1) //new one was added/removed
                {
                    trackerMax = sequences.Count - 1;
                    trackerOverrideIndex = sequences.Count - 1;
                    OnTrackerIndexChange();
                }

                if (tracker != null)
                {
                    tracker.name = gameObject.name + " - Tracker [" + trackerOverrideIndex + "]";

                    if (sequences[toTrack] != null && readyToTrack && (toTrack == trackerOverrideIndex))
                    {
                        if (sequences[toTrack].doMove)
                        {
                            sequences[toTrack].moveTo = tracker.transform.position;
                        }

                        if (sequences[toTrack].doRotate)
                        {
                            sequences[toTrack].rotateTo = tracker.transform.rotation;
                        }

                        if (sequences[toTrack].doScale)
                        {
                            sequences[toTrack].scaleTo = tracker.transform.localScale;
                        }
                    }
                }
            }
            else
            {
                trackerMax = 0;
            }
        }
    }

    /// <summary>
    /// Sequence Odin Titles
    /// </summary>
    private void OnValidate()
    {
        if(sequences != null && sequences.Count > 0)
        {
            for (int i = 0; i < sequences.Count; i++)
            {
                sequences[i].displayTitle = sequences[i].tweenTitle + " " + i;
                sequences[i].displayTitle += " [Length: " + sequences[i].tweenLength + "]";

                if (sequences[i].tweenType == SequenceTween.TweenType.Transform)
                {
                    sequences[i].isTransformMode = true;

                    if (sequences[i].doMove)
                    {
                        sequences[i].displayTitle += " | Move";
                    }
                    if (sequences[i].doRotate)
                    {
                        sequences[i].displayTitle += " | Rotate";
                    }
                    if (sequences[i].doScale)
                    {
                        sequences[i].displayTitle += " | Scale";
                    }
                }
                else
                {
                    sequences[i].isTransformMode = false;
                }

                if (sequences[i].hasInit == false)
                {
                    sequences[i].tweenTitle = "Tween";

                    if (sequences[i].moveTo == Vector3.zero)
                    {
                        if (i == 0)
                        {
                            sequences[i].moveTo = transform.position;
                        }
                        else
                        {
                            sequences[i].moveTo = sequences[i - 1].moveTo;
                        }
                    }

                    if (sequences[i].rotateTo == Quaternion.Euler(0,0,0))
                    {
                        if (i == 0)
                        {
                            sequences[i].rotateTo = transform.rotation;
                        }
                        else
                        {
                            sequences[i].rotateTo = sequences[i - 1].rotateTo;
                        }
                    }

                    if (sequences[i].scaleTo == Vector3.one)
                    {
                        if (i == 0)
                        {
                            sequences[i].scaleTo = transform.localScale;
                        }
                        else
                        {
                            sequences[i].scaleTo = sequences[i - 1].scaleTo;
                        }
                    }

                    sequences[i].hasInit = true;
                }
            }
        }
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Plays a Sequence from the list of sequences from either a specific index or the next index
    /// </summary>
    /// <param name="sequenceToPlayFrom"></param>
    public void PlaySequenceFromIndex(int sequenceToPlayFrom = -1)
    {
        isActive = true;
        //If not specified, player from the current index
        if (sequenceToPlayFrom == -1)
        {
            sequenceToPlayFrom = sequenceIndex;
        }

        //Kill the current Tween and snap to the end of the last position
        moveTweener.Kill();
        rotateTweener.Kill();
        scaleTweener.Kill();

        if (sequenceToPlayFrom > 0)
        {     
            SnapToPrevious(sequenceToPlayFrom - 1);
        }
     
        //DoTween
        if(sequences[sequenceToPlayFrom] != null)
        {
            moveTweener = DOTween.Sequence();
            rotateTweener = DOTween.Sequence();
            scaleTweener = DOTween.Sequence();

            if (sequences[sequenceToPlayFrom].tweenType == SequenceTween.TweenType.Transform)
            {
                if (sequences[sequenceToPlayFrom].doMove)
                {
                    moveTweener.Append(transform.DOMove(sequences[sequenceToPlayFrom].moveTo, sequences[sequenceToPlayFrom].tweenLength).SetEase(sequences[sequenceToPlayFrom].tweenEase).OnComplete(PlayNextSequence));
                }
                if (sequences[sequenceToPlayFrom].doRotate)
                {
                    rotateTweener.Append(transform.DORotateQuaternion(sequences[sequenceToPlayFrom].rotateTo, sequences[sequenceToPlayFrom].tweenLength).SetEase(sequences[sequenceToPlayFrom].tweenEase).OnComplete(PlayNextSequence));
                }
                if (sequences[sequenceToPlayFrom].doScale)
                {
                    scaleTweener.Append(transform.DOScale(sequences[sequenceToPlayFrom].scaleTo, sequences[sequenceToPlayFrom].tweenLength).SetEase(sequences[sequenceToPlayFrom].tweenEase).OnComplete(PlayNextSequence));
                }
            }
            else
            {
                Invoke("PlayNextSequence", sequences[sequenceToPlayFrom].tweenLength);
            }
        }
    }

    /// <summary>
    /// Pauses the current sequence
    /// </summary>
    public void PauseSequence()
    {
        moveTweener.Pause();
        rotateTweener.Pause();
        scaleTweener.Pause();
    }

    /// <summary>
    /// Resumes the current sequence
    /// </summary>
    public void ResumeSequence()
    {
        moveTweener.Play();
        rotateTweener.Play();
        scaleTweener.Play();
    }

    /// <summary>
    /// Ends the current sequence
    /// </summary>
    public void KillSequence(bool doComplete)
    {
        moveTweener.Kill(doComplete);
        rotateTweener.Kill(doComplete);
        scaleTweener.Kill(doComplete);
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Plays the next indexed sequence within the list and if we reached the end, try to loop
    /// </summary>
    private void PlayNextSequence()
    {
        sequenceIndex++;
        if (sequenceIndex < sequences.Count)
        {
            AutoPlayCurrentIndex();
        }
        else
        {
            if (!isInfinite)
            {
                timesLeftToPlay--;

                if (timesLeftToPlay > 0)
                {
                    DoLoop();
                }
            }
            else
            {
                DoLoop();
            }
        }
    }

    /// <summary>
    /// Attempt to loop the sequences
    /// </summary>
    private void DoLoop()
    {
        if (doLoop)
        {
            sequenceIndex = 0;
            AutoPlayCurrentIndex();
        }
    }

    /// <summary>
    /// Attempt to autoadvance to the next index in the sequence
    /// </summary>
    private void AutoPlayCurrentIndex()
    {
        if (doAutoAdvance)
        {
            PlaySequenceFromIndex(sequenceIndex);
        }
    }

    /// <summary>
    /// Immediately snaps to a given index in a sequence
    /// </summary>
    /// <param name="previous"></param>
    private void SnapToPrevious(int previous)
    {
        if(sequences != null && sequences.Count > 1)
        {
            if(previous > 0)
            {
                if(sequences[previous] != null)
                {
                    if (sequences[previous].tweenType == SequenceTween.TweenType.Transform)
                    {
                        if (sequences[previous].doMove)
                        {
                            transform.position = sequences[previous].moveTo;
                        }
                        if (sequences[previous].doRotate)
                        {
                            transform.rotation = sequences[previous].rotateTo;
                        }
                        if (sequences[previous].doScale)
                        {
                            transform.localScale = sequences[previous].scaleTo;
                        }
                    }                  
                }
            }
        }
    }

    /// <summary>
    /// Handles the current index the tracker is following
    /// </summary>
    private void OnTrackerIndexChange()
    {
        readyToTrack = false;
        if (tracker != null && sequences != null && sequences.Count > 0)
        {
            if (sequences[trackerOverrideIndex] != null)
            {
                tracker.transform.position = sequences[trackerOverrideIndex].moveTo;
                tracker.transform.rotation = sequences[trackerOverrideIndex].rotateTo;
                tracker.transform.localScale = sequences[trackerOverrideIndex].scaleTo;
                toTrack = trackerOverrideIndex;
            }
        }
        readyToTrack = true;
    }
    #endregion
    #endregion

    #region Classes
    [System.Serializable]
    public class SequenceTween
    {
        #region Variables & Inspector Options
        #region Top Options
        #region Tween Settings
        [BoxGroup("Tween", ShowLabel = false)]
        [HorizontalGroup("Tween/Split")]
        [BoxGroup("Tween/Split/TweenSettings", ShowLabel = false)]
        [Header("Tween Settings")]
        public TweenType tweenType = TweenType.Transform;

        [BoxGroup("Tween", ShowLabel = false)]
        [HorizontalGroup("Tween/Split")]
        [BoxGroup("Tween/Split/TweenSettings", ShowLabel = false)]
        public Ease tweenEase;

        [BoxGroup("Tween", ShowLabel = false)]
        [HorizontalGroup("Tween/Split")]
        [BoxGroup("Tween/Split/TweenSettings", ShowLabel = false)]
        public bool showDisplaySettings = false;
        #endregion

        #region Transform Options
        [BoxGroup("Tween", ShowLabel = false)]
        [HorizontalGroup("Tween/Split")]
        [ShowIfGroup("Tween/Split/isTransformMode")]
        [BoxGroup("Tween/Split/isTransformMode/TransformOption", ShowLabel = false)]
        [Header("Transform Options")]
        public bool doMove;
        [BoxGroup("Tween", ShowLabel = false)]
        [HorizontalGroup("Tween/Split")]
        [ShowIfGroup("Tween/Split/isTransformMode")]
        [BoxGroup("Tween/Split/isTransformMode/TransformOption", ShowLabel = false)]
        public bool doRotate;
        [BoxGroup("Tween", ShowLabel = false)]
        [HorizontalGroup("Tween/Split")]
        [ShowIfGroup("Tween/Split/isTransformMode")]
        [BoxGroup("Tween/Split/isTransformMode/TransformOption", ShowLabel = false)]
        public bool doScale;
        #endregion
        #endregion

        #region Middle Options
        [BoxGroup("Tween", ShowLabel = false)]
        [ShowIfGroup("Tween/showDisplaySettings")]
        [BoxGroup("Tween/showDisplaySettings/DisplaySettings", ShowLabel = false)]
        [Header("Display Settings")]
        [OnInspectorGUI]
        public string tweenTitle = "New Tween";

        [BoxGroup("Tween", ShowLabel = false)]
        [ShowIfGroup("Tween/showDisplaySettings")]
        [BoxGroup("Tween/showDisplaySettings/DisplaySettings", ShowLabel = false)]
        [ShowIf("showDisplaySettings")]
        public Color visualizeColor = Color.red;
        #endregion

        #region Bottom Options
        [BoxGroup("Tween", ShowLabel = false)]
        [BoxGroup("Tween/TweenOptions", ShowLabel = false)]
        [Header("Tween Options")]
        public float tweenLength = 1;

        [BoxGroup("Tween", ShowLabel = false)]
        [BoxGroup("Tween/TweenOptions", ShowLabel = false)]
        [ShowIf("@doMove == true && tweenType == TweenType.Transform")]
        public Vector3 moveTo = Vector3.zero;
        [BoxGroup("Tween", ShowLabel = false)]
        [BoxGroup("Tween/TweenOptions", ShowLabel = false)]
        [ShowIf("@doScale == true && tweenType == TweenType.Transform")]
        public Vector3 scaleTo = Vector3.one;
        [BoxGroup("Tween", ShowLabel = false)]
        [BoxGroup("Tween/TweenOptions", ShowLabel = false)]
        [ShowIf("@doRotate == true && tweenType == TweenType.Transform")]
        public Quaternion rotateTo = Quaternion.Euler(0,0,0);
        #endregion

        #region Stored Data
        public enum TweenType { Transform, Delay}

        [HideInInspector]
        public bool hasInit;

        [HideInInspector]
        public string displayTitle;

        [HideInInspector]
        public bool isTransformMode = true;
        #endregion
        #endregion
    }
    #endregion
}

/* HELP SECTION

Q: Nothing is showing up at all and things aren't working! Is this script totally broken???
A: Make sure you have Gizmos enables in your Unity project. If they are disabled, it wont be able to call OnDrawGizmos.

Q: The object isn't moving (and I selected that is should start on play)!
A: This is more than likely because you did not specify a Transform Option for the first sequence. If you want it to return to where it is now, exactly how it is now, check all 3 options.

Q: The object rotated once and now will not rotate back!
A: You most likely did not set the original transform rotation back in your first position

Q: When the object rotates AND scales, it becomes distorted!
A: Yeah that's a limit of Quaternions. Just try and void SKEW Rotations. Linear rotations should work fine.

Q: When I move the values of one object in the sequence, the previous values move too!
A: This is probably because you have left the default values and haven't chosen to make that sequence a Delay sequence. You can always check all 3 boxes and just not edit the values. It will learp in-place.

Q: How can I tell if a sequence is in the default state?
A: If it is titled "New Tween", it's default. Once you have done SOMETHING, it will update to be just "Tween". If it is in this default state, it won't work the way you want I promise.

*/