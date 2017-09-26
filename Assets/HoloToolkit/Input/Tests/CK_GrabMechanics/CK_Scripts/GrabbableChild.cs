﻿using UnityEngine;

/// <summary>
/// This type of grab makes the grabbed object a child of the grabber.
/// This ensures a grabbed object perfectly follows the position and rotation of the grabbing object
/// </summary>

public class GrabbableChild : BaseGrabbable
{

    public Color TouchColor;

    protected override void StartGrab(Grabber grabber1)
    {
        base.StartGrab(grabber1);
        transform.SetParent(grabber1.transform);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    protected override void EndGrab(Grabber grabber1)
    {

        base.EndGrab(grabber1);
        transform.SetParent(null);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }



    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = TouchColor;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = originalColor;
    }

    protected override void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        base.Start();
    }

    private Color originalColor;
}
