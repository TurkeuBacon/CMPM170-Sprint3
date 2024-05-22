using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    // Update is called once per frame
    void Update() {
        rb.velocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision other) {
        Destroy(gameObject);
    }
}
