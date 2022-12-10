using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class QueenHandler : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;

    [SerializeField] private Transform _target;
    [SerializeField] private ParticleSystem _teleportParticle;
    
    // Hashes for animation
    private int _isRunningHash;
    private int _isDeadHash;

    // Queen will kill butterflies
    private List<BreadButterfly> _aliveButterflies;
    [SerializeField] private float _killButterfliesDistance = 5.0f;


    IEnumerator Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _teleportParticle.Stop();
        _isRunningHash = Animator.StringToHash("isRunning");

        // Collecting butterflies to be killed
        _aliveButterflies = new List<BreadButterfly>();
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Killable");
        foreach (var butterflyObj in tmp)
        {
            _aliveButterflies.Add(butterflyObj.GetComponent<BreadButterfly>());
        }

        // Movement for crossing nav mesh link
        _agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            if (_agent.isOnOffMeshLink)
            {
                if (TargetOutsideMeshLink())
                {
                    yield return StartCoroutine(CrossAfterSeconds(_agent, 3.0f));
                }
            }
            yield return null;
        }
    }
    

    void Update()
    {
        // The target who the queen will chase
        _agent.SetDestination(_target.position);
        
        // Control the animations
        bool isRunning = _animator.GetBool(_isRunningHash);
        
        if (!isRunning && HasVelocity())
        {
            _animator.SetBool(_isRunningHash, true);
        }

        if (isRunning && !HasVelocity())
        {
            _animator.SetBool(_isRunningHash, false);
        }
        
        // Killing butterflies
        if (_aliveButterflies.Count == 0) return;
        for (int i = _aliveButterflies.Count - 1; i >= 0; i--)
        {
            BreadButterfly butterfly = _aliveButterflies[i];
            if (Vector3.Distance(transform.position, butterfly.BonePosition) < _killButterfliesDistance)
            {
                butterfly.Kill();
                _aliveButterflies.RemoveAt(i);
            }
            Debug.Log("alive num: " + _aliveButterflies.Count);
        }
    }
    

    private bool HasVelocity()
    {
        bool result = false;
        Vector3 velocity = _agent.velocity;
        
        if (velocity.sqrMagnitude > Mathf.Epsilon) {
            result = true;
        }
        return result;
    }


    private bool TargetOutsideMeshLink()
    {
        bool result = false;
        const float safeDistance = 5f;
        OffMeshLinkData data = _agent.currentOffMeshLinkData;
        Vector3 startPosition = _agent.transform.position;
        Vector3 endPosition = data.endPos;
        Vector3 targetPosition = _target.position;

        if (Vector3.Distance(startPosition, targetPosition)>safeDistance && Vector3.Distance(endPosition, targetPosition)>safeDistance)
        {
            result = true;
        }
        return result;
    }

    IEnumerator CrossAfterSeconds(NavMeshAgent agent, float delaySeconds)
    {
        if (!_teleportParticle.isPlaying) {
            _teleportParticle.Play();
        }
        yield return new WaitForSeconds(delaySeconds);
        agent.CompleteOffMeshLink();
        _teleportParticle.Stop();
    }
}
