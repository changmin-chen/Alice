using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliceVisual : MonoBehaviour
{
    public OVRCameraRig CameraRig;
    private Transform _transformReference;
    private Vector3 _localPositionOffset;

    private void Awake()
    {
        _localPositionOffset = transform.localPosition;
        _transformReference = CameraRig.centerEyeAnchor.transform;
    }

    void Update()
    {
        Vector3 localRotationRef = _transformReference.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(0f, localRotationRef.y, 0f);

        Vector3 localPositionRef = _transformReference.localPosition;
        transform.localPosition = localPositionRef + _localPositionOffset;
    }
}
