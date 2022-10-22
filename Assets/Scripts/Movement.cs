using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    // Unity components and objects
    private Rigidbody rb;
    [SerializeField] private Camera camera;
    
    // General player control parameters
    [SerializeField] private float speed = 10f;
    [SerializeField] private float gravityRotationSpeed = 10f;
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float turnSpeed = 300f;
    [SerializeField] private float jumpForce = 30f;
    
    // Normal-to-the-ground movement
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 groundNormal;  // ground normal vector which player is standing
    
    // General player control inputs
    private Vector3 verticalMovement;
    private Vector3 horizontalMovement;
    private float verticalRot;
    private float horizontalRot;

    // Flags
    private bool isGrounded;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        groundNormal = transform.up;
    }

    private void Update()
    {
        // Receive player inputs
        verticalMovement = Input.GetAxis("Vertical") * transform.forward;
        horizontalMovement = Input.GetAxis("Horizontal") * transform.right;
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
        Vector3 moveDir = (verticalMovement + horizontalMovement).normalized;
        TranslateSelf(moveDir);
    }
    private void RotateCamera(float xAngle)
    {
        xAngle = Mathf.Clamp(xAngle, -60f, 60f);
        camera.transform.localRotation = Quaternion.Euler(xAngle, 0, 0);
    }
    private void TranslateSelf(Vector3 direction)
    {
        rb.MovePosition(transform.position + Time.fixedDeltaTime * speed * direction);
    }

    private void RotateSelf(float yAngle)
    {
        transform.Rotate(0, yAngle * Time.deltaTime, 0);
    }

    private void NormalOrientation()
    {
        SetGroundNormal();  // update ground normal vector
        LerpUpRotate(groundNormal, gravityRotationSpeed);  // rotate, so the character is normal to the ground
        GroundPulling();
    }
    
    private void SetGroundNormal()
    {
        float maxRayDistance = 10f;
        RaycastHit hit;
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
        // TODO: Fix ground pulling, setting velocity is bad
        Vector3 currentVelocity = rb.velocity;
        Vector3 groundPullVelocity = -9.8f * groundNormal;
        Vector3 targetVelocity = currentVelocity + groundPullVelocity;
        rb.velocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 10f);
    }

    private void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
    
    
}
