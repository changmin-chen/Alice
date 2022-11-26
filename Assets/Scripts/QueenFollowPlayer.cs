using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class QueenFollowPlayer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [SerializeField] private Transform _player;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    void Update()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        _navMeshAgent.SetDestination(_player.position);
    }
}
