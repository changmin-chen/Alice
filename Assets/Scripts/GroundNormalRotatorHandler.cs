using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundNormalRotatorHandler : MonoBehaviour
{
    public GroundNormalRotator GroundNormalRotator;
    
    public Renderer RendererLeftHand;
        
    public Renderer RendererRightHand;
    
    public Material MaterialWhenExited;

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GroundNormalRotator.gameObject.SetActive(false);
            RendererLeftHand.material = MaterialWhenExited;
            RendererRightHand.material = MaterialWhenExited;
        }
    }
}
