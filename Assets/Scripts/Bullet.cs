using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Assigning Variables")]

    [SerializeField] private float speed = 500.0f;
    [SerializeField] private float lifeTimeDuration = 1.25f;


    private Rigidbody2D rb;

    [HideInInspector] private float f_xMinBounds;
    [HideInInspector] private float f_xMaxBounds;

    [HideInInspector] private float f_yMinBounds;
    [HideInInspector] private float f_yMaxBounds;



    private void Awake()
    {
        // Calculate the screen boundaries
        f_yMaxBounds = Camera.main.orthographicSize;
        f_yMinBounds = -Camera.main.orthographicSize;
        f_xMaxBounds = Camera.main.aspect * f_yMaxBounds;
        f_xMinBounds = -f_xMaxBounds;

        rb = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        checkBounds();
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
