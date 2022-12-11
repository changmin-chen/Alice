using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Unity.Mathematics;

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
    private float _handSpeedLeft;
    private float _handSpeedRight;
    private Vector3 _forwardDirection;
    
    [SerializeField] private float speed;
    [SerializeField] private float handMovingThreshold;
    [SerializeField] private float dragForce;

    public enum MovingMode
    {
        ConstantSpeed,
        DynamicSpeed
    }
    public MovingMode movingMode = MovingMode.ConstantSpeed;
    
    [Header("Audio")]
    [SerializeField] private AudioClip _footStepAudio;
    private AudioSource _audioSource;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        
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
        if (Time.timeSinceLevelLoad < 2.0f) return;
        
        // Get current position of the camera, left and right hands
        _currentLocalPositionLeft = LeftHand.PointerPose.position;
        _currentLocalPositionRight = RightHand.PointerPose.position;

        // Handle arm swinging movement    
        if (IsDragging()) DragMovement();
        if (IsArmSwinging())
        {
            MovePlayer();
            if (!_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(_footStepAudio);
            }
        }

        if (_rigidbody.velocity.sqrMagnitude < 0.01f)
        {
            _audioSource.Stop();
        }

        // Set previous position of the camera, left and right hands
        _previousLocalPositionLeft = _currentLocalPositionLeft;
        _previousLocalPositionRight = _currentLocalPositionRight;
    }

    private bool IsDragging()
    {
        Vector3 worldVelocity = _rigidbody.velocity;
        Vector3 localVelocity = _rigidbody.transform.InverseTransformVector(worldVelocity);
        var localHorizontalSpeed = Mathf.Abs(localVelocity.x * localVelocity.z);
        return localHorizontalSpeed > Mathf.Epsilon * Mathf.Epsilon;
    }
    
    private void DragMovement()
    {
        Vector3 dragDir = - _rigidbody.velocity;
        _rigidbody.AddForce(Time.fixedDeltaTime * dragForce * dragDir, ForceMode.Acceleration);
    }

    private bool IsArmSwinging()
    {
        Vector3 localPositionMovedLeft = _currentLocalPositionLeft - _previousLocalPositionLeft;
        Vector3 localPositionMovedRight = _currentLocalPositionRight - _previousLocalPositionRight;
        _handSpeedLeft = Mathf.Abs(localPositionMovedLeft.y);
        _handSpeedRight = Mathf.Abs(localPositionMovedRight.y);
        
        // Magnitude Check
        bool isLeftHandMoving = _handSpeedLeft > handMovingThreshold;
        bool isRightHandMoving = _handSpeedRight > handMovingThreshold;
        
        // Direction Check
        bool isReverseDirection = localPositionMovedLeft.y * localPositionMovedRight.y < 0f;

        return  isLeftHandMoving & isRightHandMoving & isReverseDirection;
    }
    
    private void MovePlayer()
    {
        // Get the forward direction bases on the direction the player is looking at
        Vector3 cameraRight = CameraRig.centerEyeAnchor.right;
        Vector3 playerUpward = transform.up;
        _forwardDirection = Vector3.Cross(cameraRight, playerUpward);
        
        // Move player base on the hand speed and the selected moving mode
        switch (movingMode)
        {
            case (MovingMode.ConstantSpeed):
                _rigidbody.velocity = speed * _forwardDirection;
                break;
            case (MovingMode.DynamicSpeed):
                float handSpeed = (_handSpeedLeft + _handSpeedRight) / 2f;
                _rigidbody.AddForce(handSpeed * speed * 200f * _forwardDirection);
                break;
        }

    }

    private void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
}
