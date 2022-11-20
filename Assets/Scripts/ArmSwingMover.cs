using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmSwingMover : MonoBehaviour
{
    // GameObjects
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject CenterEyeCamera;
    public GameObject ForwardDirection;

    // Vector3 positions
    private Vector3 PositionPreviousFrameLeftHand;
    private Vector3 PositionPreviousFrameRightHand;
    private Vector3 PlayerPositionPreviousFrame;
    private Vector3 PositionThisFrameLeftHand;
    private Vector3 PositionThisFrameRightHand;
    private Vector3 PlayerPositionThisFrame;
    
    // Speed
    public float Speed = 70;
    private float HandSpeed;
    
    void Start()
    {
        // Set original PreviousFrame positions at start up
        PlayerPositionPreviousFrame = transform.position;
        PositionPreviousFrameLeftHand = LeftHand.transform.position;
        PositionPreviousFrameRightHand = RightHand.transform.position;
    }
    
    void Update()
    {
        // Get the forward direction from the center eye camera and set it to the forward direction object
        float yRotation = CenterEyeCamera.transform.eulerAngles.y;
        ForwardDirection.transform.eulerAngles = new Vector3(0, yRotation, 0);
        
        // Get current position of hands
        PositionThisFrameLeftHand = LeftHand.transform.position;
        PositionThisFrameRightHand = RightHand.transform.position;
        
        // position of player
        PlayerPositionThisFrame = transform.position;
        
        // Get distance the hands and player has moved since the last frame
        var playerDistanceMoved = Vector3.Distance(PlayerPositionThisFrame, PlayerPositionPreviousFrame);
        var leftHandDistanceMoved = Vector3.Distance(PositionThisFrameLeftHand, PositionPreviousFrameLeftHand);
        var rightHandDistanceMoved = Vector3.Distance(PositionThisFrameRightHand, PositionPreviousFrameRightHand);
        
        // Add them up to get the handSpeed from the user minus the movement of the player to neglect the movement of the player from the
        HandSpeed = (leftHandDistanceMoved - playerDistanceMoved) + (rightHandDistanceMoved - playerDistanceMoved);
        Debug.Log("hand speed = " + HandSpeed);

        if (Time.timeSinceLevelLoad > 1f)
        {
            transform.position += HandSpeed * Speed * Time.deltaTime * ForwardDirection.transform.forward;
        }
        
        // Set previous position of hand for the next frame
        PositionPreviousFrameLeftHand = PositionThisFrameLeftHand;
        PositionPreviousFrameRightHand = PositionThisFrameRightHand;
        // Set the player position previous frame
        PlayerPositionPreviousFrame = PlayerPositionThisFrame;
    }
}
