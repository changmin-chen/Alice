using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliceVisual : MonoBehaviour
{
    public OVRManager Manager;
    public OVRCameraRig CameraRig;

    [SerializeField] private Transform[] childTransforms;
    private List<Vector3> _initialLocalRotationEulers = new List<Vector3>();

    void Awake()
    {
        Manager.usePositionTracking = false;
    }

    private void Start()
    {
        for (int i = 0; i < childTransforms.Length; ++i)
        {
            Vector3 initialRotationEuler = childTransforms[i].localRotation.eulerAngles;
            _initialLocalRotationEulers.Add(initialRotationEuler);
        }
    }


    void Update()
    {
        Vector3 cameraRotationEuler = CameraRig.centerEyeAnchor.localRotation.eulerAngles;
        for (int i = 0; i < childTransforms.Length; ++i)
        {
            Vector3 initialRotationEuler = _initialLocalRotationEulers[i];
            Vector3 newRotationEuler = cameraRotationEuler + initialRotationEuler;
            childTransforms[i].localRotation = Quaternion.Euler(initialRotationEuler.x, newRotationEuler.y, initialRotationEuler.z);
        }
    }
}
