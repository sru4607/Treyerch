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
    [TabGroup("Controls"), PropertyOrder(-1)]
    public float impactRagdollForce = 2000f;

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

    private List<Vector3> defaultRigidBodyPositions = new List<Vector3>();
    private List<Quaternion> defaultRigidBodyRotations = new List<Quaternion>();


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

        foreach(Rigidbody rigidbody in ragdollRigidBodies)
        {
            defaultRigidBodyPositions.Add(rigidbody.transform.localPosition);
            defaultRigidBodyRotations.Add(rigidbody.transform.localRotation);
        }

    }

    public void FixedUpdate()
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
                Debug.Log(realVelocity);
                ragdollTimer += Time.deltaTime;

                if(ragdollTimer >= ragdollIdleMax)
                {
                    TurnOffRagdoll();
                    ragdollTimer = 0;
                }
            }
        }
        else
        {
            ragdollTimer = 0;
            if (!readyForNextState)
            {
                if(thirdPersonCamera.isLerping == false && thirdPersonCamera.currentTarget == transform)
                {
                    ReadyNextState();
                }
            }
            else if(thirdPersonCamera.currentTarget == ragdollChest.transform && !thirdPersonCamera.isLerping)
            {
                thirdPersonCamera.StopAllCoroutines();
                thirdPersonCamera.SwapTargets(transform);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;

        //Debug.Log("Name: " + collision.collider.gameObject.name + " | Force: " + collisionForce);

        if (collisionForce >= impactRagdollForce)
        {
            if (!isRagdoll)
            {
                TurnOnRagdoll();
            }
            else
            {
                ragdollTimer = 0;
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

        ResetBonePositions();

        animator.enabled = true;

        playerCollider.isTrigger = false;
        playerRigidbody.isKinematic = false;

        characterInput.allowMovement = true;
        playerRigidbody.velocity = Vector3.zero;
    }

    public void ReadyNextState()
    {
        readyForNextState = true;
    }

    public void TeleportTo(Transform teleportPoint)
    {
        if(isRagdoll)
        {
            InstantTurnOffRagdoll();
        }

        playerRigidbody.isKinematic = true;
        ragdollChest.isKinematic = true;

        playerRigidbody.velocity = Vector3.zero;
        ragdollChest.velocity = Vector3.zero;

        playerCollider.enabled = false;

        characterInput.cc.enabled = false;

        transform.position = teleportPoint.position;
        transform.rotation = teleportPoint.rotation;

        ragdollChest.transform.localPosition = defaultRigidbodyPosition;
        ragdollChest.transform.localRotation = defaultRigidbodyRotation;

        Invoke("FinishMove", 0.2f);
    }

    private void ResetBonePositions()
    {
        for (int i = 0; i < ragdollRigidBodies.Count; i++)
        {
            ragdollRigidBodies[i].transform.localPosition = defaultRigidBodyPositions[i];
            ragdollRigidBodies[i].transform.localRotation = defaultRigidBodyRotations[i];
        }
    }

    private void FinishMove()
    {
        characterInput.cc.enabled = true;
        playerCollider.enabled = true;
        playerRigidbody.isKinematic = false;
    }

    public void InstantTurnOffRagdoll()
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
        ragdollChest.transform.localRotation = defaultRigidbodyRotation;
        ragdollChest.transform.localPosition = defaultRigidbodyPosition;

        playerRigidbody.velocity = Vector3.zero;

        foreach (Collider ragdollCollider in ragdollColliders)
        {
            ragdollCollider.isTrigger = true;
            ragdollCollider.attachedRigidbody.isKinematic = true;
        }

        thirdPersonCamera.StopAllCoroutines();
        thirdPersonCamera.SwapTargets(transform);

        ragdollChest.position = defaultRigidbodyPosition;
        ragdollChest.transform.parent = null;

        transform.position = getUpPosition;
        playerRigidbody.position = getUpPosition;

        ragdollChest.transform.parent = transform.GetChild(0);

        ResetBonePositions();

        animator.enabled = true;

        playerCollider.isTrigger = false;
        playerRigidbody.isKinematic = false;

        characterInput.allowMovement = true;
        playerRigidbody.velocity = Vector3.zero;

        readyForNextState = true;
    }
}
