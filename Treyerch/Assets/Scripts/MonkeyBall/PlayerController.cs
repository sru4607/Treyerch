using UnityEngine;
using System.Collections;
using UnityStandardAssets.Cameras;
using DG.Tweening;

public class PlayerController : MonoBehaviour 
{
	[Header("Settings")]
	public float playerSpeed = 70;

	[Header("Components")]
	public AutoCam playerCamera;	
	public Rigidbody rigidBody;
	public SphereCollider sphereCollider;

	[Header("Animation")]
	public Animator animator;
	public float scaleUpTime = 0.3f;
	public Ease scaleUpEase;
	public float spinSpeed = 2;

	[Header("Ground")]
	[Tooltip("Layers that the character can walk on")]
	public LayerMask groundLayer = 1 << 0;
	[Tooltip("Distance to became not grounded")]
	public float groundMinDistance = 0.3f;
	public bool isGrounded;

	[HideInInspector]
	public GameObject groundedObject;

	[HideInInspector]
	public bool isMovable = false;

	[HideInInspector]
	public bool goalReached = false;

	[HideInInspector]
	public static PlayerController instance; //Singleton

	[HideInInspector]
	public bool startedLaunch = false;

	private bool doneLaunch = false;
	private RaycastHit groundHit;                      // raycast to hit the ground 

	private void Awake()
	{
		instance = this;
	}

	private void Start() 
	{
		//playerCamera.SnapRotate();
		rigidBody.isKinematic = true;
		transform.localScale = Vector3.zero;
		playerCamera.InitChildCam();
	}

    private void OnEnable()
    {
		ScalePlayer();
	}

	public void ScalePlayer()
    {
		rigidBody.isKinematic = true;
		transform.localScale = Vector3.zero;
		Sequence scaleUp = DOTween.Sequence();
		scaleUp.Append(transform.DOScale(1.5f, scaleUpTime).SetEase(scaleUpEase).OnComplete(StartMovement));
	}

    private void FixedUpdate() 
	{
		CheckIsGrounded();

		if (isMovable)
		{
			Vector3 movement = Input.GetAxis("Vertical") * playerCamera.transform.forward + Input.GetAxis("Horizontal") * playerCamera.transform.right;
			movement.y = 0f;

			rigidBody.AddForce(movement * playerSpeed);

			Debug.DrawRay(transform.position, rigidBody.velocity, Color.red);
		}
		else if(goalReached)
        {
			playerCamera.followMode = true;
			rigidBody.useGravity = false;
			if (rigidBody.velocity.magnitude > 1f)
			{
				rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, Time.deltaTime);
			}
			else
            {
				if (!doneLaunch)
				{
					doneLaunch = true;
					Invoke("LaunchPlayerUp", 1.35f);
				}
            }
		}

		if(startedLaunch)
        {
			transform.Rotate(0, Vector3.up.y + (spinSpeed * Time.deltaTime), 0, Space.World);
		}
	}

	private void CheckIsGrounded()
	{
		if (sphereCollider != null)
		{
			if (Physics.Raycast(transform.position, -Vector3.up, out groundHit, sphereCollider.radius + groundMinDistance, groundLayer))
			{
				isGrounded = true;
			}
			else
            {
				isGrounded = false;
			}

			if (groundHit.collider != null)
			{
				groundedObject = groundHit.collider.gameObject;
			}
		}
	}


	private void LaunchPlayerUp()
    {
		rigidBody.velocity = Vector3.zero;
		rigidBody.isKinematic = true;
		animator.enabled = false;
		SpinLoop();
	}

	private void SpinLoop()
    {
		startedLaunch = true;
		Sequence move = DOTween.Sequence();
		move.Append(transform.DOMoveY(200, 15));
	}

	private void StartMovement()
    {
		rigidBody.isKinematic = false;
		isMovable = true;
		rigidBody.AddForce(transform.forward/25 * playerSpeed);
	}
}