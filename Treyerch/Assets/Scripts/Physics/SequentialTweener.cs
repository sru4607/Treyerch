using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SequentialTweener : MonoBehaviour
{
    [TabGroup("Sequences")]
    [ListDrawerSettings(ListElementLabelName = "displayTitle")]
    public List<SequenceTween> sequences = new List<SequenceTween>();

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

    [TabGroup("Visualize")]
    public MeshFilter meshFilter;
    [TabGroup("Visualize")]
    [ShowIf("meshFilter")]
    public bool visualizeDestinations;

    private int timesLeftToPlay = 0;
    private int sequenceIndex = 0;
    private Sequence moveTweener;
    private Sequence rotateTweener;
    private Sequence scaleTweener;

    // Start is called before the first frame update
    void Start()
    {
        timesLeftToPlay = sequenceLimit;

        if (sequences != null && sequences.Count > 0)
        {
            if (enableOnStart)
            {
                PlaySequenceFromIndex(0);
            }
        }
    }

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
    }

    private void OnValidate()
    {
        if(sequences != null && sequences.Count > 0)
        {
            for (int i = 0; i < sequences.Count; i++)
            {
                sequences[i].tweenTitle = "Tween";
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

    public void PlaySequenceFromIndex(int sequenceToPlayFrom = -1)
    {
        //If not specified, player from the current index
        if(sequenceToPlayFrom == -1)
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

    public void PauseSequence()
    {
        moveTweener.Pause();
        rotateTweener.Pause();
        scaleTweener.Pause();
    }

    public void ResumeSequence()
    {
        moveTweener.Play();
        rotateTweener.Play();
        scaleTweener.Play();
    }

    public void KillSequence()
    {
        moveTweener.Kill();
        rotateTweener.Kill();
        scaleTweener.Kill();
    }

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

    private void DoLoop()
    {
        if (doLoop)
        {
            sequenceIndex = 0;
            AutoPlayCurrentIndex();
        }
    }

    private void AutoPlayCurrentIndex()
    {
        if (doAutoAdvance)
        {
            PlaySequenceFromIndex(sequenceIndex);
        }
    }

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

    [System.Serializable]
    public class SequenceTween
    {
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

        public enum TweenType { Transform, Delay}

        [HideInInspector]
        public bool hasInit;

        [HideInInspector]
        public string displayTitle;

        [HideInInspector]
        public bool isTransformMode = true;
    }
}
