using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MazeCompletedUnityEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _completeMaze;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _completeMaze.Invoke();
        }
    }
}
