/*----------------------------------------------------------------
------------------------- alejandrodlsp --------------------------
-------------------- alejandrodlsp@hotmail.es --------------------
-----------------------------------------------------------------*/

/* 
  In Order for the script to work, your camera should be in this arrangement:

  *EmptyGameObject (With cameraFollow script attatched)
	 -EmptyGameObject	(Move this object for repositioning or rotationg the camera)
		 -Camera object;

 */


using UnityEngine;

public class cameraFollow : MonoBehaviour {

	[SerializeField]
	private Transform target;   // The target to follow

	[SerializeField]
	private bool followPlayerDefault = true; // Follows gameObject with 'Player' tag instead of target transform

	[Range(0f, 10f)]
	[SerializeField]
	private float turnSpeed = 1.5f;	// The speed in which the camera will rotate

	[SerializeField]
	private float followSpeed = 1f;	// The speed in which the camera will follow the player

	[SerializeField]	
	private float rotationSmooting = 0.0f;	// Makes the camera smooth its rotation

	[SerializeField]
	private bool cursorLocked = false;  // Lock cursor

	[SerializeField]
	private bool forceStartRotation = false; // Rotate the camera to start rotation when not moving it

	[SerializeField]
	private float forceStartRotationSpeed;

	private float yOffset;

	private float lookAngle;
	private Quaternion targetRot;
	private Quaternion startRotation;

	void Awake()
	{
		yOffset = transform.localPosition.y;
		startRotation = transform.localRotation;

		if(followPlayerDefault)	// If folow player by default
			target = GameObject.FindGameObjectWithTag("Player").transform; // Gets player with "Player" tag

		Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None; // Sets cursor lockmode
		Cursor.visible = !cursorLocked; // Sets cursor visible

		targetRot = transform.localRotation;    // Gets local rotation
	}


	void Update()
	{
		if (forceStartRotation && !Input.GetMouseButton(1) && transform.localRotation != startRotation)	// Force start rotation
		{
			goToStartRotation();
		}


		if (Input.GetMouseButton(1))
			rotate();   // Rotate


		if (cursorLocked && Input.GetMouseButtonUp(0))  //Checks for cursor locked mode updates
		{
			Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !cursorLocked;
		}
	}


	private void OnDisable()	
	{
		Cursor.lockState = CursorLockMode.None; // Disables cursor lock
		Cursor.visible = true;  // Sets cursor visible
	}

	void FixedUpdate()	// Follow the target
	{
		if (target == null)		// If we dont have a target
		{
			Debug.LogError("ameraFollow:  NO TARGET FOUND");
			return;
		}

		// If we do have a target
		Vector3 tPos = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
		transform.position = Vector3.Lerp(transform.position, tPos, followSpeed / 100);  // Follow it
	}

	private void rotate()
	{
		if (Time.timeScale < float.Epsilon)	// If we're on the right time scale
			return;

		var x = Input.GetAxis("Mouse X");	// Gets X mouse position

		lookAngle += x * turnSpeed;	// Multiplies mouse position by turning speed
		targetRot = Quaternion.Euler(0f, lookAngle, 0f);	// Creates a quaternion

		if (rotationSmooting > 0)	
		{
			transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, rotationSmooting * Time.deltaTime); // Slerp between rotation
		}
		else
		{
			transform.localRotation = targetRot; // Sets rotation
		}
	}

	private void goToStartRotation()	// moves to start rotation
	{
		transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, forceStartRotationSpeed* Time.deltaTime); // Slerp between rotation
	}
}
