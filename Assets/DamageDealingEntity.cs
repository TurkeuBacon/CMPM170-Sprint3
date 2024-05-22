using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class DamageDealingEntity : MonoBehaviour {
    public float damage;
    private Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision other) {
        GameEntity otherEntity = other.gameObject.GetComponent<GameEntity>();
        Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
        if(otherEntity && otherRb) {
            float impactMagnitude = (rb.velocity - otherRb.velocity).magnitude;
            otherEntity.dealDamage(damage * impactMagnitude * 0.5f);
        }
    }
}
