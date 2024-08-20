using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class PlayerMovement : MonoBehaviour
{
    public bool b_AllowMovement;

    [HideInInspector] float f_xMinBounds;
    [HideInInspector] float f_xMaxBounds;

    [HideInInspector] float f_yMinBounds;
    [HideInInspector] float f_yMaxBounds;

    public float f_speed = 5f;           // Constant forward speed of the aircraft
    public float f_rotationSpeed = 200f; // Speed of rotation for turning

    private Rigidbody2D rb;
    public float f_changeDirectionInterval = 2f; // Time in seconds before changing direction

    private float f_timeSinceLastChange = 0f;
    private float f_randomRotationDirection;

    public static Action<bool> OnStartbutton;

    private void OnEnable()
    {
        OnStartbutton += allowMovement;
    }

    private void OnDisable()
    {
        OnStartbutton -= allowMovement;
    }

    private void Awake()
    {
        // Calculate the screen boundaries
        f_yMaxBounds = Camera.main.orthographicSize;
        f_yMinBounds = -Camera.main.orthographicSize;
        f_xMaxBounds = Camera.main.aspect * f_yMaxBounds;
        f_xMinBounds = -f_xMaxBounds;
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Ensure no gravity affects the plane
    }

    void FixedUpdate()
    {
        checkBounds();
        if (b_AllowMovement)
        {
            HandleRotation();
        }
        else
        {
            AutoRotation();
        }
        MoveForward();
    }

    public void checkBounds()
    {
        Vector3 newPosition = transform.position;

        if (transform.position.y > f_yMaxBounds)
        {
            newPosition.y = f_yMinBounds;
        }
        else if (transform.position.y < f_yMinBounds)
        {
            newPosition.y = f_yMaxBounds;
        }

        if (transform.position.x > f_xMaxBounds)
        {
            newPosition.x = f_xMinBounds;
        }
        else if (transform.position.x < f_xMinBounds)
        {
            newPosition.x = f_xMaxBounds;
        }

        transform.position = newPosition;
    }

    void HandleRotation()
    {
        float rotationInput;
#if UNITY_EDITOR
        // Get input for rotation (left/right)
        rotationInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow keys
        rb.rotation += -rotationInput * f_rotationSpeed * Time.deltaTime;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        // Get input for rotation (left/right)
        rotationInput = CrossPlatformInputManager.GetAxis("Horizontal"); // A/D or Left/Right Arrow keys
        rb.rotation += -rotationInput * f_rotationSpeed * Time.deltaTime;
#endif
        // Apply rotation based on input
    }

    void SetRandomDirection()
    {
        // Randomly choose to turn left or right
        f_randomRotationDirection = UnityEngine.Random.Range(-1f, 1f);
    }

    void AutoRotation()
    {
        f_timeSinceLastChange += Time.deltaTime;

        // Change direction and rotation after the interval
        if (f_timeSinceLastChange >= f_changeDirectionInterval)
        {
            SetRandomDirection();
            f_timeSinceLastChange = 0f;
        }

        // Apply rotation
        rb.rotation += f_randomRotationDirection * f_rotationSpeed * Time.deltaTime;
    }

    void MoveForward()
    {
        // Always move forward in the direction the aircraft is facing
        Vector2 forward = transform.up; // "Up" is the forward direction of the sprite
        rb.velocity = forward * f_speed;
    }

    public void allowMovement(bool Move)
    {
        b_AllowMovement = Move;
    }
}
