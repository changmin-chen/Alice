using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
	public bool EnableLinearMovement = true;
	public bool EnableRotation = true;
	public bool HMDRotatesPlayer = true;
	public bool RotationEitherThumbstick = false;
	public float RotationAngle = 45.0f;
	public float Speed = 0.0f;
	public OVRCameraRig CameraRig;

	private bool ReadyToSnapTurn;
	private Rigidbody _rigidbody;
	
	private Quaternion prevHMDRot;
	private Quaternion currentHMDRot;
	
	// Arm-Swing Movement
	[SerializeField] private GameObject _leftHand;
	[SerializeField] private GameObject _rightHand;
	private Vector3 _positionPreviousFrameLeftHand;
	private Vector3 _positionPreviousFrameRightHand;
	private Vector3 _playerPositionPreviousFrame;
	private Vector3 _positionThisFrameLeftHand;
	private Vector3 _positionThisFrameRightHand;
	private Vector3 _playerPositionThisFrame;

	public event Action CameraUpdated;
	public event Action PreCharacterMove;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		if (CameraRig == null) CameraRig = GetComponentInChildren<OVRCameraRig>();
	}

	void Start ()
	{
		prevHMDRot = CameraRig.centerEyeAnchor.rotation;
		currentHMDRot = prevHMDRot;
		
		// ArmSwingMove: Set original PreviousFrame positions at start up
		_playerPositionPreviousFrame = transform.position;
		_positionPreviousFrameLeftHand = _leftHand.transform.position;
		_positionPreviousFrameRightHand = _rightHand.transform.position;
	}
	
	private void FixedUpdate()
	{
		if (CameraUpdated != null) CameraUpdated();
        if (PreCharacterMove != null) PreCharacterMove();
        
        //if (HMDRotatesPlayer) RotatePlayerToHMD();
        //if (EnableLinearMovement) StickMovement();
        if (HMDRotatesPlayer) SoftRotatePlayerToHMD();
        if (EnableLinearMovement) StickNormalMovement();
		if (EnableRotation) SnapTurn();
	}

    void RotatePlayerToHMD()
    {
		Transform root = CameraRig.trackingSpace;
		Transform centerEye = CameraRig.centerEyeAnchor;

		Vector3 prevPos = root.position;
		Quaternion prevRot = root.rotation;

		transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

		root.position = prevPos;
		root.rotation = prevRot;
    }

    void StickMovement()
	{
		Quaternion ort = CameraRig.centerEyeAnchor.rotation;
		Vector3 ortEuler = ort.eulerAngles;
		ortEuler.z = ortEuler.x = 0f;
		ort = Quaternion.Euler(ortEuler);

		Vector3 moveDir = Vector3.zero;
		Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		moveDir += ort * (primaryAxis.x * Vector3.right);
		moveDir += ort * (primaryAxis.y * Vector3.forward);
		//_rigidbody.MovePosition(_rigidbody.transform.position + moveDir * Speed * Time.fixedDeltaTime);
		_rigidbody.MovePosition(_rigidbody.position + moveDir * Speed * Time.fixedDeltaTime);
	}
    
   
    void SoftRotatePlayerToHMD()
    {
	    Transform root = CameraRig.trackingSpace;
	    Transform centerEye = CameraRig.centerEyeAnchor;

	    Vector3 prevPos = root.position;
	    Quaternion prevRot = root.rotation;
	    
	    currentHMDRot = centerEye.rotation;
	    Quaternion q = Quaternion.Inverse(centerEye.rotation);  // world-to-local quaternion
	    Quaternion localCurrentHMDRot = q * currentHMDRot;
	    Quaternion localPrevHMDRot = q * prevHMDRot;
	    Quaternion localDelta = localCurrentHMDRot * Quaternion.Inverse(localPrevHMDRot);
	    Vector3 localDeltaEuler = localDelta.eulerAngles;
	    //Debug.Log("local delta euler: " + localDeltaEuler);
	    transform.Rotate(0f, localDeltaEuler.y, 0f);
	    
	    prevHMDRot = currentHMDRot;
	    root.position = prevPos;
	    root.rotation = prevRot;
    }
	void StickNormalMovement()
	{
		Vector3 moveDir = Vector3.zero;
		Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		moveDir += primaryAxis.x * transform.right;
		moveDir += primaryAxis.y * transform.forward;
		moveDir = moveDir.normalized;
		_rigidbody.AddForce(moveDir * 10f, ForceMode.Impulse);
	}
	
	void ArmSwingMovement()
	{
		//float yRotation = CameraRig.transform.eulerAngles.y;
		
		// Get current position of hands
		_positionThisFrameLeftHand = _leftHand.transform.position;
		_positionThisFrameRightHand = _rightHand.transform.position;
	}

	void SnapTurn()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) ||
			(RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)))
		{
			if (ReadyToSnapTurn)
			{
				ReadyToSnapTurn = false;
				transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, -RotationAngle);
			}
		}
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) ||
			(RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
		{
			if (ReadyToSnapTurn)
			{
				ReadyToSnapTurn = false;
				transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, RotationAngle);
			}
		}
		else
		{
			ReadyToSnapTurn = true;
		}
	}
}
