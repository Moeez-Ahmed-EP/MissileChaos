using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 10f;           // Speed of the missile
    public float rotateSpeed = 200f;    // Speed at which the missile turns
    public float spreadTime = 0.5f;     // Time before the missile starts homing (for spread missiles)
    public float spreadDistance = 1f;   // Distance to spread before homing (for spread missiles)
    public Transform target;            // The target the missile will home towards

    private Vector2 initialDirection;   // Initial direction for spread
    private bool isHoming = false;      // Indicates if the missile is homing in on the target
    private float spreadTimer;          // Timer for the spread phase
    private bool isSpreadMissile = false; // Indicates if this missile is part of a spread

    [HideInInspector] private float f_xMinBounds;
    [HideInInspector] private float f_xMaxBounds;

    [HideInInspector] private float f_yMinBounds;
    [HideInInspector] private float f_yMaxBounds;

    public float F_DamageAmount;

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
        if (isSpreadMissile)
        {
            // Set an initial random direction for spreading
            initialDirection = (Vector2)transform.up + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            spreadTimer = spreadTime;
        }
        else
        {
            // If not a spread missile, go straight or home immediately
            isHoming = target != null;
        }
    }

    void Update()
    {
        checkBounds();

        if (isSpreadMissile && !isHoming)
        {
            // Spread phase
            transform.position += (Vector3)initialDirection * spreadDistance * Time.deltaTime;

            // Countdown to homing phase
            spreadTimer -= Time.deltaTime;
            if (spreadTimer <= 0f)
            {
                isHoming = true;
            }
        }
        else if (target != null)
        {
            // Homing phase
            Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);

            transform.position += transform.up * speed * Time.deltaTime;
        }

        // Move forward in the current direction (homing or straight)
        transform.position += transform.up * speed * Time.deltaTime;
    }

    //public void Project(Vector2 direction)
    //{
    //    rb.AddForce(direction * this.speed);

    //    Destroy(this.gameObject, this.lifeTimeDuration);
    //}

    public void SetAsSpreadMissile()
    {
        isSpreadMissile = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == target)
        {
            //Destroy(target.gameObject); // Destroy target on impact
            Destroy(gameObject); // Destroy the missile
        }
    }

    private void checkBounds()
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

        this.transform.position = newPosition;
    }
}
