using DG.Tweening;
using System;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace UnityStandardAssets.Cameras
{
    [ExecuteInEditMode]
    public class AutoCam : PivotBasedCameraRig
    {
        public float m_MoveSpeed = 99; // How fast the rig will move to keep up with target's position
        [SerializeField] private float m_TurnSpeed = 1; // How fast the rig will turn to keep up with target's rotation
        [SerializeField] private float m_RollSpeed = 0.2f;// How fast the rig will roll (around Z axis) to match target's roll.   
        [SerializeField] private float m_CurrentTurnAmount = 0.5f; // How much to turn the camera
        [SerializeField] private float minimumDistance = 0.1f; // How much velocity needed to turn the camera

        private Vector3 m_RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )
        private Quaternion rollRotation;

        [HideInInspector]
        public bool followMode = false;

        [HideInInspector]
        public Transform player;

        [HideInInspector]
        public Transform myCamera;

        [HideInInspector]
        public bool readyToTrack = false;

        private Vector3 initialCameraRotation;
        private bool lerping;

        protected override void FollowTarget(float deltaTime)
        {
            if(!readyToTrack)
            {
                return;
            }

            if (!followMode)
            {
                // if no target, or no time passed then we quit early, as there is nothing to do
                if (!(deltaTime > 0) || m_Target == null)
                {
                    return;
                }

                // initialise some vars, we'll be modifying these in a moment
                Vector3 targetForward = targetRigidbody.velocity.normalized;

                // camera position moves towards target position:
                if (!lerping)
                {
                    transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime * m_MoveSpeed);


                    if (targetForward != Vector3.zero)
                    {
                        m_RollUp = Vector3.Slerp(m_RollUp, Vector3.up, m_RollSpeed * deltaTime);

                        if (Mathf.Abs(targetRigidbody.velocity.magnitude) > 0.1f)
                        {
                            rollRotation = Quaternion.LookRotation(targetForward, m_RollUp);
                        }

                        transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, m_TurnSpeed * m_CurrentTurnAmount * deltaTime);
                    }
                }
            }
            else
            {
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-m_Target.forward, m_RollUp), m_TurnSpeed * m_CurrentTurnAmount * deltaTime);

                myCamera.rotation = Quaternion.Lerp(myCamera.rotation, Quaternion.LookRotation(player.position - myCamera.position, Vector3.up), m_TurnSpeed * deltaTime);
            }
        }

        public void InitChildCam()
        {
            myCamera = transform.GetChild(0).GetChild(0);
            initialCameraRotation = myCamera.transform.localRotation.eulerAngles;
        }

        public void EnableFollowMode()
        {
            followMode = true;
        }

        public void DisableFollowMode()
        {
            followMode = false;

            Sequence cameraRotate = DOTween.Sequence();
            cameraRotate.Append(myCamera.transform.DOLocalRotate(Quaternion.Euler(initialCameraRotation).eulerAngles, 0.5f).SetEase(Ease.InOutSine));
        }

        public void ResetCamera(bool instant = false)
        {
            lerping = true;
            readyToTrack = false;

            if (instant)
            {
                transform.rotation = Quaternion.LookRotation(m_Target.transform.forward, Vector3.up);
                transform.position = m_Target.position;
                lerping = false;
            }
            else
            {
                Sequence cameraRotate = DOTween.Sequence();
                cameraRotate.Append(transform.DORotate(Quaternion.LookRotation(m_Target.transform.forward, Vector3.up).eulerAngles, 0.5f).SetEase(Ease.InOutSine));

                Sequence cameraMove = DOTween.Sequence();
                cameraMove.Append(transform.DOMove(m_Target.position, 0.8f).SetEase(Ease.OutQuad).OnComplete(FinishLerp));
            }
        }

        private void FinishLerp()
        {
            m_RollUp = Vector3.up;
            rollRotation = transform.rotation;

            lerping = false;
            PlayerController.instance.ScalePlayer();
        }
    }
}
