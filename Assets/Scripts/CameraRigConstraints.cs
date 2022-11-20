using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigConstraints : MonoBehaviour
{
    public OVRCameraRig CameraRig;
    private Vector3 initialCenterEyeLocalPosition;
    void Awake()
    {
        initialCenterEyeLocalPosition = CameraRig.centerEyeAnchor.transform.localPosition;
    }
    
    void LateUpdate()
    {
        // Fix the relative position of the CameraRig
        CameraRig.centerEyeAnchor.transform.localPosition = initialCenterEyeLocalPosition;
    }
}
