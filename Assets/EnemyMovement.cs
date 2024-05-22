using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed, targetPlayerDist;
    private PlayerEntity player;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start() {
        player = FindObjectOfType<PlayerEntity>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        transform.LookAt(player.transform);
        Vector3 toPlayerVector = transform.position - player.transform.position;
        if(toPlayerVector.magnitude > targetPlayerDist) {
            rb.velocity = transform.rotation * Vector3.forward * speed;
        } else {
            rb.velocity = transform.rotation * Vector3.left * speed;
        }
    }
}
