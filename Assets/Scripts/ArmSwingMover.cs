using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ArmSwingMover : MonoBehaviour
{
    public OVRHand LeftHand;
    public OVRHand RightHand;
    public OVRCameraRig CameraRig;
    private Rigidbody _rigidbody;
    
    private Vector3 _previousLocalPositionLeft;
    private Vector3 _previousLocalPositionRight;
    private Vector3 _currentLocalPositionLeft;
    private Vector3 _currentLocalPositionRight;
    
    [SerializeField] private float speed;
    [SerializeField] private float handMovingThreshold;
    [SerializeField] private float dragForce;
   
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        // Set original PreviousFrame positions at start up
        _previousLocalPositionLeft = LeftHand.PointerPose.position;
        _previousLocalPositionRight = RightHand.PointerPose.position;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ReloadLevel();
    }

    private void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad < 1f) return;
        
        // Get current position of the camera, left and right hands
        _currentLocalPositionLeft = LeftHand.PointerPose.position;
        _currentLocalPositionRight = RightHand.PointerPose.position;

        // Handle arm swinging movement    
        if (IsDragging()) DragMovement();
        if (IsArmSwinging()) MovePlayer();

        // Set previous position of the camera, left and right hands
        _previousLocalPositionLeft = _currentLocalPositionLeft;
        _previousLocalPositionRight = _currentLocalPositionRight;
    }

    private bool IsDragging()
    {
        return _rigidbody.velocity.sqrMagnitude > 0f;
    }
    private void DragMovement()
    {
        Vector3 dragDir = -_rigidbody.velocity;
        _rigidbody.AddForce(dragForce * dragDir, ForceMode.Acceleration);
    }

    private bool IsArmSwinging()
    {
        Vector3 localPositionMovedLeft = _currentLocalPositionLeft - _previousLocalPositionLeft;
        Vector3 localPositionMovedRight = _currentLocalPositionRight - _previousLocalPositionRight;
        
        // Magnitude Check
        bool isLeftHandMoving = Mathf.Abs(localPositionMovedLeft.y) > handMovingThreshold;
        bool isRightHandMoving = Mathf.Abs(localPositionMovedRight.y) > handMovingThreshold;
        
        // Direction Check
        bool isReverseDirection = localPositionMovedLeft.y * localPositionMovedRight.y < 0f;

        return  isLeftHandMoving & isRightHandMoving & isReverseDirection;
    }
    
    private void MovePlayer()
    {
        // Get the forward direction bases on the direction the player is looking at
        Vector3 cameraRight = CameraRig.centerEyeAnchor.right;
        Vector3 playerUpward = transform.up;
        Vector3 moveDir = Vector3.Cross(cameraRight, playerUpward);
        
        // Move player by setting the constant speed
        _rigidbody.velocity = speed * moveDir;
    }

    private void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
}
