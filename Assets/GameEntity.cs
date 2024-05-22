using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class GameEntity : MonoBehaviour
{
    public float maxHealth, defense, invincibilityTime;
    private float invincibilityStartTime;
    protected float health;
    // Start is called before the first frame update
    void Start() {
        health = maxHealth;
    }
    
    public bool dealDamage(float damage) {
        if(Time.time - invincibilityStartTime >= invincibilityTime) {
            health = Mathf.Clamp(health-damage, 0, maxHealth);
            invincibilityStartTime = Time.time;
            return health > 0;
        }
        return false;
    }
}
