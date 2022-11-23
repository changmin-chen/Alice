using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArmSwingMover : MonoBehaviour
{
    public OVRHand LeftHand;
    public OVRHand RightHand;
    public OVRCameraRig CameraRig;

    // Vector3 positions
    private Vector3 _previousPositionLeft;
    private Vector3 _previousPositionRight;
    private Vector3 _currentPositionLeft;
    private Vector3 _currentPositionRight;

    // Speed
    [SerializeField] private float speed;
    [SerializeField] private float handSpeedThreshold;
    [SerializeField] private float dragForce;
    private float _handSpeed;
    private Rigidbody _rigidbody;
    

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        // Set original PreviousFrame positions at start up
        _previousPositionLeft = LeftHand.PointerPose.position;
        _previousPositionRight = RightHand.PointerPose.position;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }
    }

    private void FixedUpdate()
    {
        // Get current position of the camera, left and right hands
        _currentPositionLeft = LeftHand.PointerPose.position;
        _currentPositionRight = RightHand.PointerPose.position;

        // Get distance the hands and player has moved since the last frame
        var leftHandVectorMoved = _currentPositionLeft - _previousPositionLeft;
        var rightHandVectorMoved = _currentPositionRight - _previousPositionRight;
        
        // Add them up to get the handSpeed from the user minus the movement of the player to neglect the movement of the player from the
        _handSpeed = Mathf.Abs(leftHandVectorMoved.y) + Mathf.Abs(rightHandVectorMoved.y);

        // Set previous position of the camera, left and right hands
        _previousPositionLeft = _currentPositionLeft;
        _previousPositionRight = _currentPositionRight;

        if (Time.timeSinceLevelLoad < 1f) return;
        

        if (_rigidbody.velocity.sqrMagnitude > 0.01f)
        {
            DragMovement();
        }
        
        // Move the player
        var moveDir = GetForwardDirection();
        if (_handSpeed > handSpeedThreshold)
        {
            _rigidbody.velocity = speed * moveDir;
        }


    }

    private Vector3 GetForwardDirection()
    {
        Vector3 cameraRight = CameraRig.centerEyeAnchor.right;
        Vector3 playerUpward = transform.up;
        return Vector3.Cross(cameraRight, playerUpward);
    }
    private void DragMovement()
    {
        Vector3 dragDir = -_rigidbody.velocity;
        _rigidbody.AddForce(dragForce * dragDir, ForceMode.Acceleration);
    }


    private void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
    
}
