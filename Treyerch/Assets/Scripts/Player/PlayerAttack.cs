using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Reference")]
    public Transform player;
    public LayerMask hitMask;
    public bool doDrawGizmo;

    [Header("Slapping")]
    public float slapDistance;
    public float slapHeight;
    public float slapRadius;
    public float slapForce;

    private void OnDrawGizmos()
    {
        if (doDrawGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(player.position + (Vector3.up * slapHeight), slapRadius);
        }
    }

    public void SlapAction()
    {
        RaycastHit raycastHit;
        bool rayCast = Physics.SphereCast(player.position + (Vector3.up * slapHeight), slapRadius, player.forward, out raycastHit, slapDistance, hitMask);

        if (rayCast)
        {
            Rigidbody hitBody = raycastHit.transform.GetComponent<Rigidbody>();
            if (hitBody)
            {
                hitBody.AddForce(-raycastHit.normal * slapForce);
            }
        }
    }
}
