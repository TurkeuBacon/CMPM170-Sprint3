using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance;
    public LayerMask interactionLayerMask, targetLayeMask;
    public Canvas playerCanvas;
    public Transform heldTargetATransform, heldTargetBTransform;
    public float heldObjectTrackSpeedCoef, heldObjectTrackSpeedMax, heldObjectRotSpeedCoef, heldObjectRotSpeedMax, dropDist;
    public float minThrowTime, maxThowTime, maxThrowSpeed;
    public Color minThrowColor, maxThrowColor;

    private Camera playerCamera;

    private GameObject heldObjectA, heldObjectB;
    private float clickedTime;
    private bool activeA, activeB;
    public Image throwBar;
    public GameObject throwingPanel;

    // Start is called before the first frame update
    void Start() {
        playerCamera = GetComponentInChildren<Camera>();
        throwingPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        pickupDropThrow();
        updateHeldItem(ref heldObjectA, ref heldTargetATransform);
        updateHeldItem(ref heldObjectB, ref heldTargetBTransform);
    }
    private void pickupDropThrow() {
        RaycastHit hit;
        bool lookingAtItem = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance, interactionLayerMask);
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionDistance, Color.red);
        if(!activeB) {
            if(Input.GetMouseButtonDown(0)) {
                if(!heldObjectA && lookingAtItem) {
                    GameObject lookingAt = hit.collider.gameObject;
                    if(lookingAt != heldObjectB) {
                        heldObjectA = lookingAt;
                        heldObjectA.GetComponent<Interactable>().grab();
                    }
                } else if(heldObjectA) {
                    activeA = true;
                    clickedTime = Time.time;
                }
            } else if(activeA) {
                float lerpI = (Time.time - clickedTime - minThrowTime) / (maxThowTime - minThrowTime);
                float lerpIClamped = Mathf.Clamp(lerpI, 0, 1);
                if(lerpI >= 0) {
                    throwingPanel.SetActive(true);
                }
                float throwSpeed = Mathf.Lerp(0, maxThrowSpeed, lerpIClamped);
                throwBar.color = Color.Lerp(minThrowColor, maxThrowColor, lerpIClamped);
                throwBar.transform.localScale = new Vector3(lerpIClamped, 1, 1);
                if(Input.GetMouseButtonUp(0)) {
                    RaycastHit targetHit;
                    bool isTarget = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out targetHit, Mathf.Infinity, targetLayeMask);
                    Vector3 direction = isTarget ? (targetHit.point - heldObjectA.transform.position).normalized : playerCamera.transform.forward;
                    heldObjectA.GetComponent<Interactable>().drop(direction * throwSpeed);
                    heldObjectA = null;
                    throwingPanel.SetActive(false);
                    activeA = false;
                }
            }
        }
        if(!activeA) {
            if(Input.GetMouseButtonDown(1)) {
                if(!heldObjectB && lookingAtItem) {
                    GameObject lookingAt = hit.collider.gameObject;
                    if(lookingAt != heldObjectB) {
                        heldObjectB = lookingAt;
                        heldObjectB.GetComponent<Interactable>().grab();
                    }
                } else if(heldObjectB) {
                    activeB = true;
                    clickedTime = Time.time;
                }
            } else if(activeB) {
                float lerpI = (Time.time - clickedTime - minThrowTime) / (maxThowTime - minThrowTime);
                float lerpIClamped = Mathf.Clamp(lerpI, 0, 1);
                if(lerpI >= 0) {
                    throwingPanel.SetActive(true);
                }
                float throwSpeed = Mathf.Lerp(0, maxThrowSpeed, lerpIClamped);
                throwBar.color = Color.Lerp(minThrowColor, maxThrowColor, lerpIClamped);
                throwBar.transform.localScale = new Vector3(lerpIClamped, 1, 1);
                if(Input.GetMouseButtonUp(1)) {
                    RaycastHit targetHit;
                    bool isTarget = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out targetHit, Mathf.Infinity, targetLayeMask);
                    Vector3 direction = isTarget ? (targetHit.point - heldObjectB.transform.position).normalized : playerCamera.transform.forward;
                    heldObjectB.GetComponent<Interactable>().drop(direction * throwSpeed);
                    heldObjectB = null;
                    throwingPanel.SetActive(false);
                    activeB = false;
                }
            }
        }
    }
    private void updateHeldItem(ref GameObject heldObject, ref Transform heldTransform) {
        if (heldObject)
        {
            Vector3 distApart = heldTransform.position - heldObject.transform.position + new Vector3(0, 0.7f, 0);
            if (distApart.sqrMagnitude >= Mathf.Pow(dropDist, 2))
            {
                heldObject.GetComponent<Interactable>().drop();
                heldObject = null;
            }
            else
            {
                if (distApart.sqrMagnitude > Mathf.Pow(0.01f, 2))
                {
                    heldObject.GetComponent<Rigidbody>().velocity = distApart * heldObjectTrackSpeedCoef;
                    if (heldObject.GetComponent<Rigidbody>().velocity.sqrMagnitude > Mathf.Pow(heldObjectTrackSpeedMax, 2))
                    {
                        heldObject.GetComponent<Rigidbody>().velocity = distApart.normalized * heldObjectTrackSpeedMax;
                    }
                }
                else
                {
                    heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }

                Vector3 eulerAngles = heldObject.transform.localEulerAngles;
                eulerAngles = new Vector3(
                    eulerAngles.x >= 180 ? eulerAngles.x - 360 : eulerAngles.x,
                    eulerAngles.y >= 180 ? eulerAngles.y - 360 : eulerAngles.y,
                    eulerAngles.z >= 180 ? eulerAngles.z - 360 : eulerAngles.z);
                Vector3 angularDist = -eulerAngles;
                Debug.Log(angularDist + " | " + angularDist.magnitude);
                if (angularDist.sqrMagnitude > Mathf.Pow(5f, 2f))
                {
                    heldObject.GetComponent<Rigidbody>().angularVelocity = angularDist * heldObjectRotSpeedCoef;
                    if (heldObject.GetComponent<Rigidbody>().angularVelocity.sqrMagnitude > Mathf.Pow(heldObjectRotSpeedMax, 2))
                    {
                        heldObject.GetComponent<Rigidbody>().angularVelocity = angularDist.normalized * heldObjectRotSpeedMax;
                    }
                }
                else
                {
                    heldObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }
        }
    }
}
