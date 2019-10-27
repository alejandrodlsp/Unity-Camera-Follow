//////////////////////////////////////////////////////////////////
//      Player Camera Controller
//      Third person camera controller with collision detection
//      and target following.
//////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class cameraFollowWithCollision : MonoBehaviour
{
    // Input settings
    [Header("Input Settings")]
    [SerializeField] KeyCode lockCameraOrbiting = KeyCode.LeftControl;
    [SerializeField] float mouseRotationSensibility = 1f;
    [SerializeField] float mouseScrollSensibility = 1f;

    // Camera positioning settings
    [Header("Camera Positioning")]
    [SerializeField] float minDistanceFromPlayer = 1;                //min camera distance
    [SerializeField] float maxDistanceFromPlayer = 2;                //max camera distance
    float DistanceFromPlayerDelta = 0f;                              //how far the camera is from the player.
    [SerializeField] float cameraHeightFromPlayer = -2;              //how high the camera is above the player
    [SerializeField] float resumeCameraRotationSmoothing = 2f;
        
    float cameraRoationDelta = 70f;                                  //current desired angle of the camera

    [Header("Player to follow")]        public Transform target;                    //the target the camera follows
    [Header("Layer(s) to include")]     public LayerMask CamOcclusion;              //the layers that will be affected by collision

    // Camera properties
    [Header("Camera Properties")]
    public float cameraHeight = 55f;
    public float cameraPan = 0f;
    public float camRotateSpeed = 180f;
    Vector3 camPosition;
    Vector3 virtualCamMask;

    //**
    // Startup method
    void Start()
    {    
        cameraRoationDelta = target.eulerAngles.y - 45f;    //position the camera behind the target on startup.
        Cursor.lockState = CursorLockMode.Locked;           //Lock cursor.
    }

    //**
    // Update method
    void LateUpdate()
    {
        handleRotation();
    }


    //**
    //Handles camera orbiting around player, and camera follow target
    void handleRotation() {

        float HorizontalAxis = Input.GetAxis("Mouse X") * mouseRotationSensibility;

        //Offset of the targets transform (Since the pivot point is usually at the feet).
        Vector3 targetOffset = new Vector3(target.position.x, (target.position.y + 2f), target.position.z);
        Quaternion rotation = Quaternion.Euler(cameraHeight, cameraRoationDelta, cameraPan);
        Vector3 rotateVector = rotation * Vector3.one;

        //this determines where both the camera and it's mask will be.
        //the virtualCamMask is for forcing the camera to push away from walls.
        virtualCamMask =     targetOffset + Vector3.up * cameraHeightFromPlayer - rotateVector * DistanceFromPlayerDelta;
        camPosition = occludeRay(ref targetOffset); ;

        // Smooths camera movement
        transform.position = Vector3.Lerp(transform.position, camPosition, Time.deltaTime * 4f);

        // Look at target object
        transform.LookAt(target);

        // Wrap the cam orbit rotation
        if (cameraRoationDelta > 360)
            cameraRoationDelta = 0f;
        else if (cameraRoationDelta < 0f)
            cameraRoationDelta = (cameraRoationDelta + 360f);

        // Control camera orbiting locking
        if (!Input.GetKey(lockCameraOrbiting)) {
            Quaternion rot = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            target.rotation = Quaternion.Lerp(target.rotation, rot, resumeCameraRotationSmoothing);
        }
        DistanceFromPlayerDelta -= Input.GetAxis("Mouse ScrollWheel") * mouseScrollSensibility;

        // 
        cameraRoationDelta += HorizontalAxis * camRotateSpeed * Time.deltaTime;
        DistanceFromPlayerDelta = Mathf.Clamp(DistanceFromPlayerDelta, minDistanceFromPlayer, maxDistanceFromPlayer);
    }


    //**
    // Handles camera collision detection
    Vector3 occludeRay(ref Vector3 targetFollow)
    {
        //declare a new raycast hit.
        RaycastHit wallHit = new RaycastHit();
        //linecast from your player (targetFollow) to your cameras mask (camMask) to find collisions.
        if (Physics.Linecast(targetFollow, virtualCamMask, out wallHit, CamOcclusion))
        {
            //the x and z coordinates are pushed away from the wall by hit.normal.
            //the y coordinate stays the same.
            return new Vector3(wallHit.point.x + wallHit.normal.x * 0.5f, virtualCamMask.y, wallHit.point.z + wallHit.normal.z * 0.5f);
        }
        else return virtualCamMask;
    }
}
