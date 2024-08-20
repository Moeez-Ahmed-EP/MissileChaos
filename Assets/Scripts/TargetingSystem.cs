using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetingSystem : MonoBehaviour
{
    public Transform currentClosestTarget;
    public Transform currentSelectedTarget;
    public List<Transform> rangedTargets;
    public float lockOnRange = 10f; // Maximum distance to lock on to a target
    public LayerMask targetLayer;   // Layer where targets are located
    public static Action<Transform> OnHouseDestroy;

    

    Vector2 V2_TouchPosition;


    private void OnEnable()
    {
        OnHouseDestroy += removeHouse;
    }

    private void OnDisable()
    {
        OnHouseDestroy -= removeHouse;
    }

    public void removeHouse(Transform t)
    {
        if (rangedTargets.Count > 0)
        {
            if (rangedTargets.Contains(t))
                rangedTargets.Remove(t);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Mouse Button Pressed");
            DetectObjectClick();
        }
#endif


#if UNITY_ANDROID && !UNITY_EDITOR
        getTouchPosition();
        DetectObjectByTouch();
#endif

        LockOnTarget();
    }

    void DetectObjectClick()
    {
        // Convert the mouse position to world point
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Perform a 2D raycast to detect the clicked object
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            // Log or handle the clicked object
            Debug.Log("Clicked on object: " + hit.collider.gameObject.name);

            if (hit.collider.gameObject.tag == "House")
            {
                // Perform any action on the clicked object
                if (currentSelectedTarget == null)
                {
                    HandleClick(hit.collider.gameObject);
                }
            }
        }
    }

    void DetectObjectByTouch()
    {
        // Convert the mouse position to world point
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(V2_TouchPosition);

        // Perform a 2D raycast to detect the clicked object
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            // Log or handle the clicked object
            Debug.Log("Clicked on object: " + hit.collider.gameObject.name);

            if (hit.collider.gameObject.tag == "House")
            {
                // Perform any action on the clicked object
                if (currentSelectedTarget == null)
                {
                    HandleClick(hit.collider.gameObject);
                }
            }
        }
    }

    void HandleClick(GameObject clickedObject)
    {
        float distance = Vector2.Distance(transform.position, clickedObject.transform.position);

        if (distance <= lockOnRange)
        {
            currentSelectedTarget = clickedObject.transform;
            currentSelectedTarget.GetComponent<House>().setCrossairState(true);
            Debug.Log("Target selected: " + currentSelectedTarget.name);
        }
        else
        {
            Debug.Log("Clicked object is out of range.");
        }
    }

    public void getTouchPosition()
    {
        // Check if there is at least one touch on the screen
        if (Input.touchCount > 0)
        {
            // Get the first touch (index 0)
            Touch touch = Input.GetTouch(0);

            // Get the position of the touch
            Vector2 touchPosition = touch.position;

            // Handle each phase of the touch
            switch (touch.phase)
            {
                case TouchPhase.Ended:
                    // This is called when the user lifts their finger off the screen
                    Debug.Log("Touch ended at position: " + touchPosition);
                    V2_TouchPosition = touchPosition;
                    break;

                case TouchPhase.Canceled:
                    // This is called when the system cancels a touch (e.g., due to a system interruption)
                    Debug.Log("Touch was canceled");
                    V2_TouchPosition = touchPosition;
                    break;
            }
        }
    }


    void LockOnTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, lockOnRange, targetLayer);

        foreach (Transform target in rangedTargets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance > lockOnRange)
            {
                //closestDistance = distance;
                target.GetComponent<House>().setCrossairState(false);
                //currentSelectedTarget = null;
            }
        }



        rangedTargets.Clear();

        if (targets.Length > 0)
        {
            // Select the closest target
            float closestDistance = Mathf.Infinity;
            Collider2D closestTarget = null;

            foreach (Collider2D target in targets)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
                rangedTargets.Add(target.transform);
            }

            if (closestTarget != null)
            {
                if (currentSelectedTarget != null)
                {
                    if (rangedTargets.Contains(currentSelectedTarget))
                    {
                        currentClosestTarget = currentSelectedTarget;
                    }
                }
                else
                {
                    //currentSelectedTarget = closestTarget.transform;
                    currentClosestTarget = closestTarget.transform;
                }

                foreach (Transform target in rangedTargets)
                {
                    if (target != currentClosestTarget)
                    {
                        target.GetComponent<House>().setCrossairState(false);
                    }
                    else
                    {
                        target.GetComponent<House>().setCrossairState(true);
                    }
                }
                //rangedTargets.Add(currentTarget);
                //Debug.Log("Target locked on: " + currentClosestTarget.name);
            }

            foreach (Transform target in rangedTargets)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance > lockOnRange)
                {
                    currentSelectedTarget = null;
                }
            }
        }
        else
        {
            currentClosestTarget = null;
            if (!rangedTargets.Contains(currentSelectedTarget))
                currentSelectedTarget = null;
            //Debug.Log("No target in range.");
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);
    }
}