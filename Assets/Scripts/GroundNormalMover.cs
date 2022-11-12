using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundNormalMover : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _trans;

    // Normal-to-the-ground movement
    [SerializeField] private float gravityRotationSpeed = 10f;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 groundNormal;  // ground normal vector which player is standing
    
    void Start()
    {
        _rb = transform.root.GetComponent<Rigidbody>();
        _trans = transform.root;

        _rb.useGravity = false;
    }
    
    void Update()
    {
        NormalOrientation();
    }
    
    private void NormalOrientation()
    {
        SetGroundNormal();  // update ground normal vector
        LerpUpRotate(groundNormal, gravityRotationSpeed);  // rotate, so the character is normal to the ground
        GroundPulling();  // using attraction force from the ground, instead of using gravity
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
    
    private void LerpUpRotate(Vector3 targetDir, float lerpSpeed)
    {
        Vector3 currentDir = transform.up;
        Vector3 lerpDir = Vector3.Lerp(currentDir, targetDir, Time.deltaTime * lerpSpeed);
        _trans.rotation = Quaternion.FromToRotation(currentDir, lerpDir) * _trans.rotation;
    }
    
    private void GroundPulling()
    {
        Vector3 currentVelocity = _rb.velocity;
        Vector3 groundPullVelocity = - groundNormal;
        Vector3 targetVelocity = currentVelocity + groundPullVelocity;
        _rb.velocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 10f);
    }
}
