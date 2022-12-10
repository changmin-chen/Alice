using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadButterfly : MonoBehaviour
{

    public Vector3 BonePosition { get; private set; }
    
    [SerializeField]
    private Transform _boneTransform;
    
    private Rigidbody _rigidbody;
    private Animator _animator;
    
    private int _isDeadHash = Animator.StringToHash("isDead");

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        BonePosition = _boneTransform.position;
    }

    public void Kill()
    {
        _rigidbody.useGravity = true;
        _animator.SetBool(_isDeadHash, true);
    }
}
