using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float damage;
    private BulletEffect bulletEffect;
    private float lifetime = 30f;
    private float elapsedTime = 0f;
    
    private bool isActive = false;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isActive) return;

        if (rb != null)
            rb.linearVelocity = direction * speed;
        
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lifetime)
            Destroy(gameObject);
    }

    public void Initialize(Vector2 direction, float speed, float damage, BulletEffect bulletEffect)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.damage = damage;
        this.bulletEffect = bulletEffect;
        isActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        if (collision.CompareTag("Enemy"))
        {
            EnemyCombat enemyCombat = collision.GetComponent<EnemyCombat>();
            if (enemyCombat != null)
            {
                enemyCombat.EnemyTakeDamage((int)damage);
                Debug.Log($"탄막 hit! {damage}");
            }
            Destroy(gameObject);
        }

        if (collision.CompareTag("Player") && bulletEffect == BulletEffect.Heal)
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(-1);
                Debug.Log("플레이어 회복!");
            }
            Destroy(gameObject);
        }
    }
}