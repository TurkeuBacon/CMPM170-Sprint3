using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyEntity : GameEntity
{
    public TextMeshPro healthReadout;
    public float fireRate;
    public GameObject projectilePrefab;
    private PlayerEntity player;
    private float lastShotTime;
    void Start() {
        health = maxHealth;
        player = FindObjectOfType<PlayerEntity>();
        lastShotTime = Time.time;
    }

    // Update is called once per frame
    void Update() {
        healthReadout.text = "Health: " + Mathf.Round(health * 10f) / 10f;
        healthReadout.transform.LookAt(player.GetComponentInChildren<Camera>().transform);
        if(Time.time - lastShotTime >= fireRate) {
            lastShotTime = Time.time;
            Instantiate(projectilePrefab, transform.position + transform.forward * 1.5f, transform.rotation);
        }
        if(health == 0) {
            DestroyImmediate(gameObject);
        }
    }
}
