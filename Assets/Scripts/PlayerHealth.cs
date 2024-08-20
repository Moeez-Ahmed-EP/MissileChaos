using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Collider2D cmp_Collider;
    [SerializeField] GameObject gb_Explosion;
    [SerializeField] GameObject gb_Sprite;
    [SerializeField] GameObject gb_Shadow;
    [SerializeField] PlayerMovement cmp_Movement;
    [SerializeField] Rigidbody2D rb;
    public int i_CurrentHealth;
    public int i_MaxHealth;
    public static Action respawnPlayer { get; set; }

    private void OnEnable()
    {
        respawnPlayer += Respawn;

    }

    private void OnDisable()
    {
        respawnPlayer -= Respawn;
    }

    private void Awake()
    {
        cmp_Movement = GetComponent<PlayerMovement>();
        rb= GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Bullet collided with player");
            Die();
            Destroy(collision.gameObject);
        }
    }

    public void Die()
    {
        rb.velocity = Vector3.zero;
        cmp_Movement.enabled = false;
        gb_Explosion.SetActive(true);
        gb_Sprite.SetActive(false);
        gb_Shadow.SetActive(false);
        i_CurrentHealth--;
        Invoke(nameof(showGameOverScreen), .5f);
    }

    public void showGameOverScreen()
    {
        GameManager.OnPlayerDeath(i_CurrentHealth);
    }

    public void Respawn()
    {
        StartCoroutine(RespawnProcess());
    }

    public IEnumerator RespawnProcess()
    {
        yield return new WaitForSeconds(.5f);
        cmp_Movement.enabled = true;
        gb_Sprite.SetActive(true);
        gb_Explosion.SetActive(false);
        yield return new WaitForSeconds(3.0f);
    }
}
