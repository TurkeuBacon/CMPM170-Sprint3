using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Description (:
    A one-handed Physics based physics based character controller.
    Only uses the mouse, with all instances of keyboard input existing
    solely for playtesting sessions and debuggig.

    Created for the course "CSE 171: Game Design Studio I" at UCSC.
*/

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class WASDPlayerController : BasePlayerController {

    private Rigidbody rb;


    
    // MOVEMENT
    [Header("Movment")]
    public float walkingSpeed;
    public float walkingAcceleration, walkingDeceleration, inAirDecelCoef;
    
    // JUMPING
    [Header("Jumping")]
    public float jumpHeight;
    public float gravity, maxStepHeight, stepClimbSpeed;
    [SerializeField]
    private bool jumping, wasStep;
    
    // USER INPUT
    private bool jumpFlag;
    private Vector2 moveInput;

    private Vector3 startingPosition;

    void Start() {
        rb = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = GetComponentInChildren<Camera>();

        jumpFlag = false;
        moveInput = Vector2.zero;
        startingPosition = transform.position;
    }

    void Update() {
        if(movementEnabled) {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            jumpFlag = true;
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            transform.position = startingPosition;
        }
        if(Input.GetKeyDown(KeyCode.R)) {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
        if(Input.GetKeyDown(KeyCode.Q)) {
            Application.Quit();
        }

        updateCamera();
    }

    void FixedUpdate() {
        grounded = isGrounded();
        Vector3 currentVelocity = rb.velocity;

        addWalkingMovement(ref currentVelocity);
        addVerticalMovement(ref currentVelocity);

        rb.velocity = currentVelocity;
    }

    private void addWalkingMovement(ref Vector3 currentVelocity) {
        Vector3 currentVelocityXZ = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        bool decelerateOverride = false;
        bool moving = moveInput.sqrMagnitude != 0;
        Debug.Log(moveInput);
        if(moving) {
            float lastMagnitude = currentVelocityXZ.magnitude;
            currentVelocityXZ += transform.rotation * new Vector3(moveInput.x, 0f, moveInput.y) * walkingAcceleration * Time.fixedDeltaTime;
            if(lastMagnitude < walkingSpeed) {
                if(currentVelocityXZ.magnitude > walkingSpeed) {
                    currentVelocityXZ = currentVelocityXZ.normalized * walkingSpeed;
                }
            } else {
                decelerateOverride = true;
                currentVelocityXZ = currentVelocityXZ.normalized * lastMagnitude;
            }
        }
        if((!moving || decelerateOverride) && currentVelocityXZ.magnitude > 0f) {
            Vector3 decelDir = -currentVelocityXZ.normalized;
            currentVelocityXZ += decelDir * walkingDeceleration * (grounded ? 1f : inAirDecelCoef) * Time.fixedDeltaTime;
            if(currentVelocityXZ.sqrMagnitude <= 0.01f || Vector3.Dot(currentVelocityXZ, decelDir) >= 0) {
                currentVelocityXZ = Vector3.zero;
            }
        }

        currentVelocity = new Vector3(currentVelocityXZ.x, currentVelocity.y, currentVelocityXZ.z);
    }
    
    private bool checkForStep() {
        RaycastHit hit;
        Debug.DrawRay(transform.position + Vector3.up * 0.01f, transform.rotation * Vector3.forward * 0.6f, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * maxStepHeight, transform.rotation * Vector3.forward * 0.6f, Color.red);
        if(Physics.Raycast(transform.position + Vector3.up * 0.01f, transform.rotation * Vector3.forward, out hit, 0.6f, groundCheckLayerMask)) {
            if(Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) >= 0.2) {
                return false;
            }
            Interactable Interactable = hit.collider.GetComponent<Interactable>();
            if(Interactable && Interactable.grabbed) return false;
            if(Physics.Raycast(transform.position + Vector3.up * maxStepHeight, transform.rotation * Vector3.forward, 0.6f, groundCheckLayerMask)) {
                return false;
            }
            return true;
        }
        return false;
    }
    private void addVerticalMovement(ref Vector3 currentVelocity) {
        if(jumpFlag) {
            tryJump(ref currentVelocity);
        }
        if(!grounded) {
            if(jumping && currentVelocity.y <= 0) {
                jumping = false;
            }
            currentVelocity += Vector3.down * gravity * Time.fixedDeltaTime;
        } else if(!jumping) {
            currentVelocity.y = -0.1f;
        }
        if(checkForStep() && moveInput.y > 0) {
            wasStep = true;
            Debug.Log("STEP");
            currentVelocity.y = stepClimbSpeed;
        } else if(wasStep) {
            wasStep = false;
            currentVelocity.y = currentVelocity.y / 4;
        }
    }
    private void tryJump(ref Vector3 currentVelocity) {
        if(grounded && !jumping) {
            currentVelocity += Vector3.up * Mathf.Sqrt(2 * gravity * jumpHeight);
            jumping = true;
        }
        jumpFlag = false;
    }
    
    private void updateCamera() {
        Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 rotationDelta = new Vector3(-mouseMovement.y, mouseMovement.x, 0f) * cameraSensitivity * Time.deltaTime;

        float currentPlayerRotationY = transform.localRotation.eulerAngles.y;
        currentPlayerRotationY += rotationDelta.y;
        transform.localRotation = Quaternion.Euler(Vector3.up * currentPlayerRotationY);

        float currentCameraRotationX = playerCamera.transform.localRotation.eulerAngles.x;
        if(currentCameraRotationX >= 180) currentCameraRotationX -= 360;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX + rotationDelta.x, -verticalRange/2, verticalRange/2);
        playerCamera.transform.localRotation = Quaternion.Euler(Vector3.right * currentCameraRotationX);
    }

    public void setMovementEnabled(bool enabled) {
        movementEnabled = enabled;
    }

}
