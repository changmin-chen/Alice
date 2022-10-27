using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    // Unity components and objects
    private Rigidbody rb;
    [SerializeField] private Camera firstPersonCamera;
    
    // General player control parameters
    [SerializeField] private float speed = 10f;
    [SerializeField] private float gravityRotationSpeed = 120f;
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float turnSpeed = 300f;
    [SerializeField] private float jumpForce = 10f;
    
    // Normal-to-the-ground movement
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 groundNormal;  // ground normal vector which player is standing
    
    // General player control inputs
    private Vector3 forwardMovement;
    private Vector3 rightMovement;
    private Vector3 movement;
    private float verticalRot;
    private float horizontalRot;

    // Flags
    private bool isGrounded;
    
    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        rb.useGravity = false;
        rb.transform.parent = null;

        groundNormal = transform.up;
    }

    private void Update()
    {
        // Follow rigidbody-child position
        transform.position = rb.position;  

        // Receive player inputs
        forwardMovement = Input.GetAxis("Vertical") * transform.forward;
        rightMovement = Input.GetAxis("Horizontal") * transform.right;
        movement = forwardMovement + rightMovement;
        verticalRot -= Input.GetAxis("Mouse Y") * sensitivity;
        horizontalRot = Input.GetAxis("Mouse X") * turnSpeed;

        // Keydown events
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(jumpForce * transform.up, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }
        
        // Process player inputs
        RotateCamera(verticalRot);
        RotateSelf(horizontalRot);
        
        // Normal-to-the-ground orientation
        NormalOrientation();

    }
    
    private void FixedUpdate()
    {
        TranslateSelf(movement);
    }
    
    private void RotateCamera(float xAngle)
    {
        xAngle = Mathf.Clamp(xAngle, -60f, 60f);
        firstPersonCamera.transform.localRotation = Quaternion.Euler(xAngle, 0, 0);
    }
    
    private void TranslateSelf(Vector3 direction)
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = direction * speed;
        rb.velocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 4f);
    }

    private void RotateSelf(float yAngle)
    {
        transform.Rotate(0, yAngle * Time.deltaTime, 0);
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
        transform.rotation = Quaternion.FromToRotation(currentDir, lerpDir) * transform.rotation;
    }
    
    private void GroundPulling()
    {
        // TODO: Too high ground pull velocity will cause the normal movement mechanics to be failed
        Vector3 currentVelocity = rb.velocity;
        Vector3 groundPullVelocity = - groundNormal;
        Vector3 targetVelocity = currentVelocity + groundPullVelocity;
        rb.velocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 10f);
    }

    private void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
    
    
}
