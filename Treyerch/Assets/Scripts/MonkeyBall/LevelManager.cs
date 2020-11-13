using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour 
{
	[TabGroup("Level Properties")]
	[Header("Timer")]
	public int totalStageSeconds = 99;
	[TabGroup("Level Properties")]
	[Header("Next Stage")]
	public string nextSceneToLoad = "MainMenu";

	[TabGroup("References")]
	[Header("Timer")]
	public List<GoalTrigger> allLevelTimers = new List<GoalTrigger>();
	[TabGroup("References")]
	[Header("Fallout")]
	[SerializeField]
	private PlayerFallOutDetector falloutTrigger;

	[TabGroup("Tilt")]
	public Transform cameraTransform;

	[TabGroup("Tilt")]
	public MeshCollider levelGeometry;

	[TabGroup("Tilt")]
	[SerializeField]
	private float speed = 50;

	[TabGroup("Tilt")]
	[SerializeField]
	private float maxRotationAngle = 10;

	[TabGroup("Tilt")]
	[SerializeField]
	private float rotationResetSpeed = 5;

	private Quaternion _originalRotation;

	[HideInInspector]
	public float timeLeft;

	[HideInInspector]
	public bool timeRanOut = false;

	[TabGroup("Post Processing")]
	public float postProcessLerp = 3f;
	[TabGroup("Post Processing")]
	public PostProcessVolume ppVolume;
	[TabGroup("Post Processing")]
	public PostProcessVolume ppPresentVolume;

	private DepthOfField depthOfFieldLayer = null;
	private DepthOfField depthOfFieldPresentLayer = null;


	void Start() 
	{
		_originalRotation = transform.localRotation;
		CenterOnChildred(transform);

		timeLeft = totalStageSeconds;
		UpdateTimer();
		GetPostProcessInfo(true);
	}

	void FixedUpdate() 
	{
		if (PlayerController.instance && PlayerController.instance.presentCamera.gameObject.activeSelf == false)
		{
			if (!PlayerController.instance.goalReached && timeLeft > 0)
			{
				timeLeft -= Time.deltaTime;

				if (timeLeft <= 0)
				{
					timeRanOut = true;

					if (UIController.instance)
					{
						UIController.instance.ResetScore();
					}

					timeLeft = 0;
					falloutTrigger.TriggerFallout();
				}

				UpdateTimer();
			}

			float moveHorizontal = 0;
			float moveVertical = 0;

			if (PlayerController.instance.isMovable)
			{
				moveHorizontal = Input.GetAxis("Horizontal");
				moveVertical = Input.GetAxis("Vertical");
			}
			else if (!PlayerController.instance.goalReached)
			{
				timeLeft = totalStageSeconds;
				UpdateTimer();
			}

			Vector3 movement = -Input.GetAxis("Horizontal") * cameraTransform.forward + Input.GetAxis("Vertical") * cameraTransform.right;
			movement.y = 0f;

			if (moveHorizontal == 0 && moveVertical == 0)
			{
				transform.localRotation = Quaternion.Lerp(transform.localRotation, _originalRotation, Time.deltaTime * rotationResetSpeed);
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
			}
			else if (PlayerController.instance && PlayerController.instance.isMovable)
			{
				transform.Rotate(movement * speed * Time.deltaTime);

				float x = transform.localEulerAngles.x;
				float z = transform.localEulerAngles.z;

				transform.localEulerAngles = new Vector3(ClampAngle(x, -maxRotationAngle, maxRotationAngle), 0, ClampAngle(z, -maxRotationAngle, maxRotationAngle));
			}
		}
	}

	public void CenterOnChildred(Transform aParent)
	{
		var childColliders = levelGeometry.transform.GetComponentsInChildren<Collider>();

		Vector3 pos = Vector3.zero;

		foreach (var Col in childColliders)
		{
			pos += Col.bounds.center;
		}

		//pos += levelGeometry.bounds.center;

		Vector3 currentGlobalPos = levelGeometry.transform.position;

		var childs = aParent.Cast<Transform>().ToList();
		foreach (var C in childs)
		{
			C.parent = null;
		}

		aParent.position = pos;

		foreach (var C in childs)
			C.parent = aParent;
		
		//levelGeometry.transform.position = currentGlobalPos;
	}

	public void DoDOFFadeIn()
    {
		StartCoroutine(PostProcessDOFFade(depthOfFieldLayer, depthOfFieldPresentLayer, 6.75f, true));
	}

	public void DoDOFFadeOut()
	{
		StartCoroutine(PostProcessDOFFade(depthOfFieldLayer, depthOfFieldPresentLayer, 500f, true));
	}

	private void GetPostProcessInfo(bool doReset)
	{
		if (ppVolume)
		{
			ppVolume.profile.TryGetSettings(out depthOfFieldLayer);
		}

		if (ppPresentVolume)
		{
			ppPresentVolume.profile.TryGetSettings(out depthOfFieldPresentLayer);
		}

		if (doReset)
		{
			StartCoroutine(PostProcessDOFFade(depthOfFieldLayer, depthOfFieldPresentLayer, 500f, false));
		}
	}

	private IEnumerator PostProcessDOFFade(DepthOfField layer, DepthOfField layer2, float lerpValue, bool doLerp = true)
	{
		if (doLerp)
		{
			float elapsedTime = 0;
			while (elapsedTime < postProcessLerp)
			{
				layer.focusDistance.value = Mathf.Lerp(layer.focusDistance.value, lerpValue, (elapsedTime / postProcessLerp));
				layer2.focusDistance.value = Mathf.Lerp(layer2.focusDistance.value, lerpValue, (elapsedTime / postProcessLerp));

				elapsedTime += Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			layer.focusDistance.value = lerpValue;
			layer2.focusDistance.value = lerpValue;
		}
	}

	public void LoadNextStage()
    {
		SceneManager.LoadScene(nextSceneToLoad);
    }

	private void UpdateTimer()
    {
		string[] currentTimer = FormatTime(timeLeft).Split(':');

		foreach(GoalTrigger goal in allLevelTimers)
        {
			goal.mainTimer.text = currentTimer[0];
			goal.secondaryTimer.text = currentTimer[1];
		}
	}

	private string FormatTime(float time)
	{
		int intTime = (int)time;
		int minutes = intTime / 60;
		int seconds = (intTime % 60) + (minutes * 60);
		float fraction = time * 100;
		fraction %= 100;
		string timeText = String.Format("{0:000}:{1:00}", seconds, fraction);
		return timeText;
	}


	public void ResetWorldTilt()
    {
		transform.localRotation =  _originalRotation;
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
	}

	private float ClampAngle(float angle, float min, float max) 
	{		
		if (angle < 90 || angle > 270) 
		{
			if (angle > 180) 
			{
				angle -= 360;
			}

			if (max > 180) 
			{
				max -= 360;
			}

			if (min > 180) 
			{
				min -= 360;
			}
		}    

		angle = Mathf.Clamp(angle, min, max);

		if (angle < 0)
		{
			angle += 360;
		}

		return angle;
	}
}