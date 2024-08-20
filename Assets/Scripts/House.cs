using UnityEngine;

public class House : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Sprite[] images;
    [SerializeField] SpriteRenderer cmp_Renderer;
    [SerializeField] GameObject gb_crossair;
    [SerializeField] GameObject gb_Explosion;

    [SerializeField] float f_CurrentHealth;
    [SerializeField] float f_MaxHealth;

    public HouseType E_CurrentHouseType;

    [SerializeField] Transform t_Health;
    [SerializeField] Collider2D collider2D;

    void Start()
    {
        collider2D = GetComponent<Collider2D>();    
        //f_MaxHealth = (int)E_CurrentHouseType * 100;
        f_CurrentHealth = f_MaxHealth;
        if (images.Length > 0)
        {
            cmp_Renderer.sprite = images[Random.Range(0,images.Length)];
        }

        updateHealthUI();
    }

    public void updateHealthUI()
    {
        if (f_CurrentHealth > 0)
        {
            float percent = f_CurrentHealth / f_MaxHealth;
            t_Health.transform.localScale = new Vector3(percent,1,1);
        }
        else
        {
            t_Health.transform.localScale = new Vector3(0,1,1);
        }
    }

    public void setCrossairState(bool state)
    {
        gb_crossair.SetActive(state);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Hit bullet");
            f_CurrentHealth -= collision.GetComponent<Missile>().F_DamageAmount;
            updateHealthUI();
            if (f_CurrentHealth <= 0)
            {
                collider2D.enabled = false;
                givePlayerScore();
                cmp_Renderer.gameObject.SetActive(false);
                gb_Explosion.SetActive(true);
                setCrossairState(false);
                //Destroy(gameObject);
                Invoke(nameof(disabelHouse), .5f);
            }
            Destroy(collision.gameObject);  
        }
    }

    public void givePlayerScore()
    {
        switch (E_CurrentHouseType)
        {
            case HouseType.easy:
                GameManager.OnEnemyDeath(100);
                break;
            case HouseType.medium:
                GameManager.OnEnemyDeath(200);
                break;
            case HouseType.hard:
                GameManager.OnEnemyDeath(300);
                break;
            default:
                break;
        }
    }

    public void disabelHouse()
    {
        GameManager.OnHouseDestroyed();
        TargetingSystem.OnHouseDestroy(transform);
        Destroy(gameObject);
    }
}
