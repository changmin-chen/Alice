using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndgameTrigger : MonoBehaviour
{
    [SerializeField] private LevelLoader _levelLoader;
    [SerializeField] private GameObject _player;

    private Rigidbody _rigidbody;
    private ArmSwingMover _armSwingMover;

    [SerializeField] private float _fallingDownSpeed;
    [SerializeField] private float _fallingDownSeconds;

    private void Start()
    {
        _armSwingMover = _player.GetComponent<ArmSwingMover>();
        _rigidbody = _player.GetComponent<Rigidbody>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(EndgameSequence());
        }
    }

    IEnumerator EndgameSequence()
    {
        // Disable movement
        _armSwingMover.enabled = false;
        
        // Drop down by constant velocity
        _rigidbody.useGravity = false;
        _rigidbody.velocity = _fallingDownSpeed * Vector3.down;
        
        // Wait for seconds
        yield return new WaitForSeconds(_fallingDownSeconds);

        // Load next level
        _levelLoader.LoadNextLevel();
    }
}
