using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundNormalRotater : MonoBehaviour
{
    private Rigidbody _rigidbody;

    // Normal-to-the-ground movement
    [SerializeField] private float gravityRotationSpeed = 10f;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 groundNormal;  // ground normal vector which player is standing

    public OVRCameraRig CameraRig;
    [SerializeField] private bool isRotaterFollowCamera;
    
    void Start()
    {
        _rigidbody = transform.root.GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (isRotaterFollowCamera) RotaterFollowCamera();  // TODO the function is buggy
        SetGroundNormal();  // update ground normal vector
        StandOnGroundRotation(groundNormal, gravityRotationSpeed);  // rotate, so the character is normal to the ground
        GroundPulling();  // using attraction force from the ground, instead of using gravity
    }

    private void RotaterFollowCamera()
    {
        Quaternion rot = CameraRig.centerEyeAnchor.rotation;
        transform.rotation = rot;
    }

    private void SetGroundNormal()
    {
        RaycastHit hit;
        float maxRayDistance = 10f;
        groundNormal = transform.up;
        
        for (int i = 0; i < groundChecks.Length; i++)
        {
            Vector3 origin = groundChecks[i].position;
            Vector3 direction = -groundChecks[i].transform.up;
            if (Physics.Raycast(origin, direction, out hit, maxRayDistance, groundLayer))
            {
                groundNormal += hit.normal;
            }
        }
        groundNormal = groundNormal.normalized;
    }
    
    private void StandOnGroundRotation(Vector3 targetDir, float lerpSpeed)
    {
        Vector3 currentDir = transform.up;
        Vector3 lerpDir = Vector3.Lerp(currentDir, targetDir, Time.deltaTime * lerpSpeed);
        _rigidbody.MoveRotation(Quaternion.FromToRotation(currentDir, lerpDir) * _rigidbody.rotation);
    }
    
    private void GroundPulling()
    {
        Vector3 groundPullDir = -groundNormal;
        _rigidbody.AddForce(Time.fixedDeltaTime * groundPullDir, ForceMode.Acceleration);
    }
}
