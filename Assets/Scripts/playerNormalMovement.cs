using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class playerNormalMovement : MonoBehaviour {

    public float moveSpeed = 12;
    public float turnSpeed = 240;
    public float lerpSpeed = 10;
    public float gravity = 50; //gravity acceleration
    public float jumpSpeed = 10;
    public float deltaGrounded = 0.2f; //player is grounded upto this distance
    public float jumpRage = 2; //range to detect target wall
    public float mouseSensitivty = 5.0f;

    private Vector3 surfaceNormal; //current surface normal
    private Vector3 myNormal; //player's normal
    private float distGround; //distance between player position to ground
    private float verticalRotation = 0; //for mouse look
    private Ray ray;
    private RaycastHit hit;
    private Transform gravityCenter;

    private Rigidbody myRB;
    private BoxCollider myCollider;
    private Camera camera;

    public bool isJumping = false;
    public bool isGrounded = false;
    public bool reached = false;

    // Use this for initialization
    void Start () {

        myRB = GetComponent<Rigidbody>();
        myCollider = GetComponent<BoxCollider>();
        camera = Camera.main;

        myNormal = transform.up;
       // myRB.useGravity = false;
        myRB.freezeRotation = true; //disable physics rotation
        distGround = myCollider.bounds.extents.y - myCollider.center.y;

       
	}


	void FixedUpdate()
    {
        myRB.AddForce(-gravity * myRB.mass * myNormal);
    }

	// Update is called once per frame
	void Update () {

        //jump to wall or simple jump

        if (isJumping) return; //abort update while jumping



        if (Input.GetButtonDown("Jump") && isGrounded)
        {

            Debug.DrawRay(hit.point, hit.normal, Color.red, 2.0f);
            myRB.AddForce(hit.normal * 1000);
            /*
            ray = new Ray(transform.position, transform.forward);
            if(Physics.Raycast(ray, out hit, jumpRage))
            {
                StartCoroutine(JumpToWall(hit.point, hit.normal));
            }
            else if(isGrounded) //player is grounded. do normal jump
            {
                
            }
            */

        }

        //movement code
        transform.Rotate(0, Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime, 0);
        //camera y movement
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivty;
        verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60.0f) ;
        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        //update surface normal and isGrounded
        ray = new Ray(transform.position, -myNormal); //cast ray downwards
        if(Physics.Raycast(ray, out hit))
        {
            isGrounded = hit.distance <= distGround + deltaGrounded;
            surfaceNormal = hit.normal;
        }
        else
        {
            isGrounded = false;
            surfaceNormal = Vector3.up;
        }

        myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
        Vector3 myForward = Vector3.Cross(transform.right, myNormal);
        Quaternion targetRot = Quaternion.LookRotation(myForward, myNormal);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed * Time.deltaTime);
        transform.Translate(0, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
            

       }

    public IEnumerator JumpToWall(Vector3 point, Vector3 normal)
    {
        Vector3 initialPos = camera.transform.localPosition;
        
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 150, 0.2f);
        isJumping = true;
        myRB.isKinematic = true;
        Vector3 orgPos = transform.position;
        Quaternion orgRot = transform.rotation;
        Vector3 destPos = point + normal * (distGround + 0.5f); //will jump to 0.5 above wall
        Vector3 myForward = Vector3.Cross(transform.right, normal);
        Quaternion destRot = Quaternion.LookRotation(myForward, normal);

        for(float t = 0.0f; t < 1.0; )
        {
            camera.transform.localPosition = initialPos + Random.insideUnitSphere * 0.7f;
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(orgPos, destPos, t);
            transform.rotation = Quaternion.Slerp(orgRot, destRot, t);
            yield return null;
            // next frame start here
        }
        
        myNormal = normal;
        myRB.isKinematic = false;
        isJumping = false;
        reached = true;
    }




}
