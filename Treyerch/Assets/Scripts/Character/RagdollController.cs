using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Invector.vCharacterController;
using DG.Tweening;

public class RagdollController : MonoBehaviour
{
    [TabGroup("Controls"), PropertyOrder(-1)]
    [ReadOnly]
    public bool isRagdoll = true;
    [TabGroup("Controls"), PropertyOrder(-1)]
    public float ragdollIdleMax = 3f;

    /*
    [TabGroup("Controls"), PropertyOrder(-1)]
    [Button(ButtonSizes.Medium)]
    private void GetRagdollComponents()
    {
        ragdollRigidBodies = new List<Rigidbody>(gameObject.GetComponentsInChildren<Rigidbody>());
        ragdollColliders = new List<Collider>();

        foreach (Rigidbody foundRigidBody in ragdollRigidBodies)
        {
            if (foundRigidBody.gameObject.GetComponent<Collider>())
            {
                ragdollColliders.Add(foundRigidBody.gameObject.GetComponent<Collider>());
            }
        }
    }
    */

    [TabGroup("Controls")]
    [Button(ButtonSizes.Medium), PropertyOrder(-1)]
    public void ToggleRagdoll()
    {
        if (isRagdoll)
        {
            TurnOffRagdoll();
        }
        else
        {
            TurnOnRagdoll();
        }
    }

    [TabGroup("References"), PropertyOrder(0)]
    public vThirdPersonCamera thirdPersonCamera;
    [TabGroup("References"), PropertyOrder(0)]
    public Rigidbody ragdollChest;
    [TabGroup("References"), PropertyOrder(0)]
    public Animator animator;
    [TabGroup("References"), PropertyOrder(0)]
    public Rigidbody playerRigidbody;
    [TabGroup("References"), PropertyOrder(0)]
    public Collider playerCollider;
    [TabGroup("References"), PropertyOrder(0)]
    public vThirdPersonInput characterInput;

    [TabGroup("Stored Componenets"), PropertyOrder(1)]
    public List<Rigidbody> ragdollRigidBodies = new List<Rigidbody>();
    [TabGroup("Stored Componenets"), PropertyOrder(1)]
    public List<Collider> ragdollColliders = new List<Collider>();

    private Vector3 defaultRigidbodyPosition;
    private Quaternion defaultRigidbodyRotation;

    private Vector3 getUpPosition;
    private bool readyForNextState = true;
    private float realVelocity;
    private Vector3 positionLastFrame;
    private float ragdollTimer;

    public void Start()
    {
        defaultRigidbodyPosition = ragdollChest.transform.localPosition;
        defaultRigidbodyRotation = ragdollChest.transform.localRotation;
    }

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            if (readyForNextState && !isRagdoll && thirdPersonCamera.target == transform)
            {
                readyForNextState = false;
                TurnOnRagdoll();
            }
        }

        if(isRagdoll)
        {
            realVelocity = Mathf.Abs(Vector3.Distance(ragdollChest.position, positionLastFrame)) / Time.deltaTime;
            positionLastFrame = ragdollChest.position;

            if(realVelocity < 0.1f)
            {
                ragdollTimer += Time.deltaTime;

                if(ragdollTimer >= ragdollIdleMax)
                {
                    TurnOffRagdoll();
                    ragdollTimer = 0;
                }
            }
        }
    }

    public void TurnOnRagdoll()
    {
        isRagdoll = true;
        characterInput.cc.Sprint(false);
        Vector3 currentVelocity = playerRigidbody.velocity;
        defaultRigidbodyPosition = ragdollChest.transform.localPosition;
        defaultRigidbodyRotation = ragdollChest.transform.localRotation;

        thirdPersonCamera.StartCoroutine(thirdPersonCamera.CameraChangeDelay(0, 2f, ragdollChest.transform));

        playerRigidbody.isKinematic = true;
        playerCollider.isTrigger = true;

        animator.enabled = false;
        characterInput.allowMovement = false;

        foreach (Collider ragdollCollider in ragdollColliders)
        {
            ragdollCollider.isTrigger = false;
            ragdollCollider.attachedRigidbody.isKinematic = false;
        }

        ragdollChest.velocity = currentVelocity * 8;
        readyForNextState = true;
    }

    public void TurnOffRagdoll()
    {
        isRagdoll = false;
        getUpPosition = ragdollChest.transform.position;

        RaycastHit raycastHit;
        // raycast for check the ground distance
        if (Physics.Raycast(ragdollChest.transform.position, Vector3.down, out raycastHit, 1, characterInput.cc.groundLayer))
        {
            getUpPosition = raycastHit.point;
        }

        ragdollChest.isKinematic = true;
        Sequence transition = DOTween.Sequence();
        transition.Append(ragdollChest.transform.DOLocalRotateQuaternion(defaultRigidbodyRotation, 0.3f));
        transition.Append(ragdollChest.transform.DOLocalMove(defaultRigidbodyPosition + transform.InverseTransformPoint(getUpPosition), 0.3f).OnComplete(FinishTurnOff));
    }

    public void FinishTurnOff()
    {
        playerRigidbody.velocity = Vector3.zero;

        foreach (Collider ragdollCollider in ragdollColliders)
        {
            ragdollCollider.isTrigger = true;
            ragdollCollider.attachedRigidbody.isKinematic = true;
        }

        thirdPersonCamera.StartCoroutine(thirdPersonCamera.CameraChangeDelay(0.1f, 0.2f, transform));

        ragdollChest.position = defaultRigidbodyPosition;
        ragdollChest.transform.parent = null;

        transform.position = getUpPosition;
        playerRigidbody.position = getUpPosition;

        ragdollChest.transform.parent = transform.GetChild(0);
    
        animator.enabled = true;

        playerCollider.isTrigger = false;
        playerRigidbody.isKinematic = false;

        characterInput.allowMovement = true;
        playerRigidbody.velocity = Vector3.zero;

        readyForNextState = true;
    }
}
