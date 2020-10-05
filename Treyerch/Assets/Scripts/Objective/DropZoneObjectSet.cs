using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class DropZoneObjectSet : MonoBehaviour
{
    #region Variables & Inspector Options
    #region Bob
    [Title("Bob Settings")]
    public float bobSpeed = 0;
    public Axis bobAxis = Axis.Y;
    public Vector2 bopClamp;
    public Ease bopEase;
    #endregion

    #region Rotate
    [Title("Rotate Settings")]
    public float rotateSpeed = 0;
    public Axis rotationAxis = Axis.X;
    public Ease rotationEase;
    #endregion

    #region Scale
    [Title("Scale Settings")]
    public float scaleSpeed = 0;
    public Axis scaleAxis = Axis.All;
    public Vector2 scaleClamp;
    public Ease scaleEase;
    #endregion

    #region Stored Data
    public enum Axis { X, Y, Z, All }

    private Sequence moveSequence;
    private Sequence scaleSequence;
    private Sequence rotateSequence;

    private bool moveFirst = true;
    private bool rotateFirst = true;
    private bool scaleFirst = true;

    private Vector3 setPos;
    private Quaternion setRot;
    private Vector3 setScale;
    #endregion
    #endregion

    public void Start()
    {
        setPos = transform.localPosition;
        setRot = transform.localRotation;
        setScale = transform.localScale;

        DoMoveAnimate();
        DoRotateAnimate();
        DoScaleAnimate();
    }

    private void DoMoveAnimate()
    {
        moveSequence = DOTween.Sequence();

        if(bobSpeed > 0)
        {
            if (moveFirst)
            {
                if (bobAxis == Axis.Y)
                {
                    moveSequence.Join(transform.DOLocalMoveY(bopClamp.y, bobSpeed).SetEase(bopEase));
                }
                else if (bobAxis == Axis.X)
                {
                    moveSequence.Join(transform.DOLocalMoveX(bopClamp.y, bobSpeed).SetEase(bopEase));
                }
                else if (bobAxis == Axis.Z)
                {
                    moveSequence.Join(transform.DOLocalMoveZ(bopClamp.y, bobSpeed).SetEase(bopEase));
                }
                else if (bobAxis == Axis.All)
                {
                    moveSequence.Join(transform.DOLocalMove(new Vector3(bopClamp.y, bopClamp.y, bopClamp.y), bobSpeed).SetEase(bopEase));
                }
            }
            else
            {
                if (bobAxis == Axis.Y)
                {
                    moveSequence.Join(transform.DOLocalMoveY(bopClamp.x, bobSpeed).SetEase(bopEase));
                }
                else if (bobAxis == Axis.X)
                {
                    moveSequence.Join(transform.DOLocalMoveX(bopClamp.x, bobSpeed).SetEase(bopEase));
                }
                else if (bobAxis == Axis.Z)
                {
                    moveSequence.Join(transform.DOLocalMoveZ(bopClamp.x, bobSpeed).SetEase(bopEase));
                }
                else if (bobAxis == Axis.All)
                {
                    moveSequence.Join(transform.DOLocalMove(new Vector3(bopClamp.x, bopClamp.x, bopClamp.x), bobSpeed).SetEase(bopEase));
                }
            }
        }

        moveFirst = !moveFirst;
        moveSequence.OnComplete(DoMoveAnimate);
    }

    private void DoRotateAnimate()
    {
        rotateSequence = DOTween.Sequence();

        if (rotateSpeed > 0)
        {
            Vector3 rotateVector = new Vector3();
            int rotateValue = 180;

            if(!rotateFirst)
            {
                rotateValue = 360;
            }

            if (rotationAxis == Axis.X)
            {
                rotateVector = new Vector3(rotateValue, 0, 0);
            }
            else if (rotationAxis == Axis.Y)
            {
                rotateVector = new Vector3(0, rotateValue, 0);
            }
            else if (rotationAxis == Axis.Z)
            {
                rotateVector = new Vector3(0, 0, rotateValue);
            }
            else if (rotationAxis == Axis.All)
            {
                rotateVector = new Vector3(rotateValue, rotateValue, rotateValue);
            }

            rotateSequence.Join(transform.DOLocalRotate(rotateVector, rotateSpeed).SetEase(rotationEase));
        }

        rotateFirst = !rotateFirst;
        rotateSequence.OnComplete(DoRotateAnimate);
    }

    private void DoScaleAnimate()
    {
        scaleSequence = DOTween.Sequence();

        if (scaleSpeed > 0)
        {
            if (scaleFirst)
            {
                if (scaleAxis == Axis.Y)
                {
                    scaleSequence.Join(transform.DOScaleY(scaleClamp.y, scaleSpeed).SetEase(scaleEase));
                }
                else if (scaleAxis == Axis.X)
                {
                    scaleSequence.Join(transform.DOScaleX(scaleClamp.y, scaleSpeed).SetEase(scaleEase));
                }
                else if (scaleAxis == Axis.Z)
                {
                    scaleSequence.Join(transform.DOScaleZ(scaleClamp.y, scaleSpeed).SetEase(scaleEase));
                }
                else if (scaleAxis == Axis.All)
                {
                    scaleSequence.Join(transform.DOScale(scaleClamp.y, scaleSpeed).SetEase(scaleEase));
                }
            }
            else
            {
                if (scaleAxis == Axis.Y)
                {
                    scaleSequence.Join(transform.DOScaleY(scaleClamp.x, scaleSpeed).SetEase(scaleEase));
                }
                else if (scaleAxis == Axis.X)
                {
                    scaleSequence.Join(transform.DOScaleX(scaleClamp.x, scaleSpeed).SetEase(scaleEase));
                }
                else if (scaleAxis == Axis.Z)
                {
                    scaleSequence.Join(transform.DOScaleZ(scaleClamp.x, scaleSpeed).SetEase(scaleEase));
                }
                else if (scaleAxis == Axis.All)
                {
                    scaleSequence.Join(transform.DOScale(scaleClamp.x, scaleSpeed).SetEase(scaleEase));
                }
            }    
        }

        scaleFirst = !scaleFirst;
        scaleSequence.OnComplete(DoScaleAnimate);
    }

    public void SetGameObject(GameObject toSet)
    {
        DropZoneObject foundDZO = toSet.GetComponent<DropZoneObject>();
        if(foundDZO)
        {
            foundDZO.enabled = false;
        }

        Rigidbody foundBody = toSet.GetComponent<Rigidbody>();
        if(foundBody)
        {
            foundBody.isKinematic = true;
        }

        moveSequence.Kill();
        rotateSequence.Kill();
        scaleSequence.Kill();

        toSet.transform.parent = transform.parent;

        moveSequence = DOTween.Sequence();
        moveSequence.Join(toSet.transform.DOLocalMove(setPos, 0.5f));
        moveSequence.Join(toSet.transform.DOLocalRotateQuaternion(setRot, 0.5f));
        moveSequence.Join(toSet.transform.DOScale(setScale, 0.5f));
    }
}
