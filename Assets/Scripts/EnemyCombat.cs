using UnityEditor;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public float attackRange = 0.8f;
    public float knockbackForce = 20f;
    public float stunTime = 0.2f;
    public int damageAmount = 2;
    public float attackCooldown = 2f;
    public float attackDuration = 0.5f;
    
    private float lastAttackTime = -999f; // 마지막 공격 시간
    public Transform attackPoint;
    private LayerMask playerLayer;
    private bool isAttacking = false;
    private float attackStartTime = 0f;
    private bool canAttack;

    void Start()
    {
        attackPoint = transform.Find("AttackPoint");
        playerLayer = LayerMask.GetMask("Player");
        
        Debug.Log($"EnemyCombat initialized. Player layer mask: {playerLayer.value}");
    }

    void Update()
    {
        // 공격 진행 중인지 확인
        if (isAttacking && Time.time >= attackStartTime + attackDuration)
        {
            isAttacking = false;
            Debug.Log("Attack animation finished");
        }
    }

    // 공격 가능 여부 확인
    public bool CanAttack()
    {
        canAttack = Time.time >= lastAttackTime + attackCooldown;
        if (!canAttack)
        {
            Debug.Log($"Attack on cooldown. Time left: {(lastAttackTime + attackCooldown) - Time.time:F2}s");
        }
        return canAttack;
    }

    // 공격이 완료되었는지 확인
    public bool IsAttackFinished()
    {
        return !isAttacking;
    }

    public void Attack()
    {
        // 쿨다운 체크
        if (!CanAttack())
        {
            return;
        }
        
        // 공격 시작
        isAttacking = true;
        attackStartTime = Time.time;
        lastAttackTime = Time.time;
        
        Debug.Log("=== ATTACK STARTED ===");

        // AttackPoint 위치 결정
        Vector2 attackPos = attackPoint.position;
        
        Debug.Log($"Attack position: {attackPos}");
        Debug.Log($"Attack range: {attackRange}");
        
        // OverlapCircle로 플레이어 감지 (훨씬 간단!)
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRange, playerLayer);
        
        Debug.Log($"Circle hits: {hits.Length}");
        
        // 히트된 모든 객체 처리
        foreach (Collider2D hit in hits)
        {
            Debug.Log($"Hit object: {hit.name} (Layer: {LayerMask.LayerToName(hit.gameObject.layer)})");
            
            // 체력 감소
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log($"Dealing {damageAmount} damage to {hit.name}!");
                playerHealth.ChangeHealth(-damageAmount);
            }
            else
            {
                Debug.LogWarning($"PlayerHealth component not found on {hit.name}");
            }
            
            // 넉백 적용
            Player playerMovement = hit.GetComponent<Player>();
            if (playerMovement != null)
            {
                Debug.Log($"Applying knockback! Force: {knockbackForce}");
                playerMovement.KnockBack(transform, knockbackForce, stunTime);
            }
            else
            {
                Debug.LogWarning($"Player component not found on {hit.name}");
            }
        }
        
        if (hits.Length == 0)
        {
            Debug.LogWarning("No hits detected! Check layer settings and attack range.");
        }
    }
    
    // 디버그용 Gizmo
    private void OnDrawGizmos()
    {
        Vector2 attackPos = attackPoint.position;
        
        // 공격 범위를 빨간 원으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
        
        // 중심점 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos, 0.1f);
    }
}
