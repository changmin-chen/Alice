using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class QueenHandler : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    [SerializeField] private Transform _target;
    private int _isRunningHash;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _isRunningHash = Animator.StringToHash("isRunning");
    }
    
    void Update()
    {
        // The target who the queen will chase
        _navMeshAgent.SetDestination(_target.position);
        
        // Control the animations
        bool isRunning = _animator.GetBool(_isRunningHash);
        
        if (!isRunning && CheckIsRunning())
        {
            _animator.SetBool(_isRunningHash, true);
        }

        if (isRunning && !CheckIsRunning())
        {
            _animator.SetBool(_isRunningHash, false);
        }
    }
    

    private bool CheckIsRunning()
    {
        bool result = false;
        Vector3 velocity = _navMeshAgent.velocity;
        
        if (velocity.sqrMagnitude > Mathf.Epsilon) {
            result = true;
        }
        return result;
    }
}
