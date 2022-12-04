using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AliceCollisionHandler : MonoBehaviour
{
    // Collision to Queen
    [SerializeField] private float _levelLoadDelay = 2.0f;
    
    // Collision to Checkpoint
    [SerializeField] private GroundNormalRotator _groundNormalRotator;
    [SerializeField] private Renderer _rendererLeftHand;
    [SerializeField] private Renderer _rendererRightHand;

    private Material _originalMaterial;
    
    private void Awake()
    {
        _originalMaterial = _rendererLeftHand.material;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel(); // TODO remove this after finishing game dev
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Queen")
        {
            //StartQueenCatchAliceSequence();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            _groundNormalRotator.gameObject.SetActive(false);
            _rendererLeftHand.material = _originalMaterial;
            _rendererRightHand.material = _originalMaterial;
        }
    }

    private void StartQueenCatchAliceSequence()
    {
        GetComponent<ArmSwingMover>().enabled = false;
        Invoke(nameof(ReloadLevel), _levelLoadDelay);
        RecoverGravityDirection();
    }

    private void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    private void RecoverGravityDirection()
    {
        float gravityForce = Physics.gravity.magnitude;
        Physics.gravity = gravityForce * Vector3.down;
    }
}
