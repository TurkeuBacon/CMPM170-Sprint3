using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerController : MonoBehaviour
{

    // CAMERA CONTROL
    [Header("Camera Control")]

    public float cameraSensitivity;
    public float verticalRange;
    protected Camera playerCamera;

    public float groundCheckDistance;
    public LayerMask groundCheckLayerMask;

    protected bool grounded;
    protected Vector3[] groundCheckDirections = {
        Vector3.down,
        new Vector3(1, -1, 0).normalized,
        new Vector3(0, -1, 1).normalized,
        new Vector3(-1, -1, 0).normalized,
        new Vector3(0, -1, -1).normalized,
    };

    // MOVEMENT TUTORIAL
    protected bool movementEnabled = true;

    protected bool isGrounded() {
        bool validHit = false;
        for(int i = 0; i < groundCheckDirections.Length; i++) {
            Vector3 direction = groundCheckDirections[i];
            Vector3 origin = transform.position + Vector3.up * 0.5f + direction * 0.4f;
            Debug.DrawRay(origin, direction * (groundCheckDistance + 0.1f), Color.red);
            RaycastHit hit;
            if(Physics.Raycast(origin, direction, out hit, groundCheckDistance + 0.1f, groundCheckLayerMask)) {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable && interactable.grabbed) continue;
                validHit = true;
            }
        }
        return validHit;
    }

    public bool getGrounded(){
        return grounded;
    }

    public void updateSensitivity(float newSens) {
        cameraSensitivity = newSens;
    }
}
