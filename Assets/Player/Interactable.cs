using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{

    public PhysicMaterial grabbedPhysicsMat;
    public bool grabbed;
    private PhysicMaterial normalPhysicsMat;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabbed = false;
    }

    public void grab()
    {
        rb.mass = 0;
        normalPhysicsMat = GetComponent<Collider>().material;
        GetComponent<Collider>().material = grabbedPhysicsMat;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        grabbed = true;
    }
    public void drop(Vector3 throwV)
    {
        rb.useGravity = true;
        rb.mass = 1;
        grabbed = false;
        GetComponent<Collider>().material = normalPhysicsMat;
        rb.velocity = transform.rotation * throwV;
    }
    public void drop() {
        drop(Vector3.zero);
    }
}
