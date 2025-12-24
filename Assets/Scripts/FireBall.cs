using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 5.0f; // 초당 5칸 (160px)
    public int damage = 1;     // 플레이어 체력 1 깎음

    private Vector2 moveDir;

    public void Setup(Vector2 dir)
    {
        moveDir = dir.normalized;
        // 5초 뒤 자동 삭제 (화면 밖으로 나가면 삭제 처리 대용)
        Destroy(gameObject, 5.0f);
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 충돌 시
        if (other.CompareTag("Player"))
        {
            // 플레이어 체력 깎는 코드 (PlayerHealth.cs의 TakeDamage 호출)
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject); // 투사체도 파괴? 
            Destroy(gameObject);
        }
    }
}