using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public ElementType bombElement = ElementType.Water; // 기본값: 물 (변경 가능)
    public int baseDamage = 200; // 기획서 기준 데미지 200 [cite: 6]
    public float timeToExplode = 2.0f;
    public float explosionRadius = 1.5f;
    public GameObject explosionEffect;

    void Start() { StartCoroutine(ExplodeSequence()); }

    IEnumerator ExplodeSequence()
    {
        yield return new WaitForSeconds(timeToExplode);
        Explode();
    }

    void Explode()
    {
        if (explosionEffect != null) Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            // 1. 보스 타격 (속성 정보 전달)
            BossHealth boss = hit.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(baseDamage, bombElement); // ★ 속성 전달!
            }

            // 2. 장애물 파괴
            if (hit.CompareTag("Obstacle")) Destroy(hit.gameObject);
        }
        Destroy(gameObject);
    }
}