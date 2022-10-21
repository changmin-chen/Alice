using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Camera camera;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float gravityRotationSpeed = 10f;
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float turnSpeed = 300f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private LayerMask groundLayer;
    
    
    private Vector3 verticalMovement;
    private Vector3 horizontalMovement;
    private float verticalRot;
    private float horizontalRot;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // get inputs
        verticalMovement = Input.GetAxis("Vertical") * transform.forward;
        horizontalMovement = Input.GetAxis("Horizontal") * transform.right;
        verticalRot -= Input.GetAxis("Mouse Y") * sensitivity;
        horizontalRot = Input.GetAxis("Mouse X") * turnSpeed;

        RotateCamera();
        RotateSelf();
        ProcessNormalMovement();
        
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(jumpForce * transform.up, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }
        
    }
    
    private void FixedUpdate()
    {
        TranslateSelf();
    }
    private void RotateCamera()
    {
        verticalRot = Mathf.Clamp(verticalRot, -60f, 60f);
        camera.transform.localRotation = Quaternion.Euler(verticalRot, 0, 0);
    }
    private void TranslateSelf()
    {
        Vector3 moveDir = (verticalMovement + horizontalMovement).normalized;
        rb.MovePosition(transform.position + Time.fixedDeltaTime * speed * moveDir);
    }

    private void RotateSelf()
    {
        transform.Rotate(0, horizontalRot * Time.deltaTime, 0);
    }

    private void ProcessNormalMovement()
    {
        NormalizeOrientation();

        // TODO: Fix ground pulling, setting velocity is bad
        Vector3 groundNormal = GetGroundNormal();
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = -groundNormal * 10f;
        rb.velocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 10f);
    }

    private void NormalizeOrientation()
    {
        Vector3 groundNormal = GetGroundNormal();
        Vector3 lerpDir = Vector3.Lerp(transform.up, groundNormal, Time.deltaTime * gravityRotationSpeed);
        transform.rotation = Quaternion.FromToRotation(transform.up, lerpDir) * transform.rotation;
    }

    private Vector3 GetGroundNormal()
    {
        RaycastHit hitFront;
        RaycastHit hitCentre;
        RaycastHit hitBack;
        Physics.Raycast(groundChecks[0].position, -groundChecks[0].transform.up, out hitFront, 10f, groundLayer);
        Physics.Raycast(groundChecks[1].position, -groundChecks[1].transform.up, out hitCentre, 10f, groundLayer);
        Physics.Raycast(groundChecks[2].position, -groundChecks[2].transform.up, out hitBack, 10f, groundLayer);

        Vector3 hitDir = transform.up;
        if (hitFront.transform is not null) { hitDir += hitFront.normal; }
        if (hitCentre.transform is not null) { hitDir += hitCentre.normal; }
        if (hitBack.transform is not null) { hitDir += hitBack.normal; }
        return hitDir.normalized;
    }

    private void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
    
    
}
