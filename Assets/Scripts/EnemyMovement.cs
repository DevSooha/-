using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyCombat enemyCombat;
    private Transform player;
    private EnemyState enemyState;
    private float facingDirection = -1;
    
    [SerializeField] private Transform detectionPoint;
    public float movespeed = 2f;
    public float attackRange = 1f;
    public float detectRange = 5f;
    public LayerMask playerLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyCombat = GetComponent<EnemyCombat>();
        
        if (detectionPoint == null)
        {
            detectionPoint = transform.Find("DetectionPoint");
        }
        
        ChangeState(EnemyState.Idle);
        
        Debug.Log($"EnemyMovement initialized. Player layer mask: {playerLayer.value}");
    }

    void Update()
    {
        // 공격 중이 아니거나 공격이 끝났으면 플레이어 체크
        if (enemyState != EnemyState.Attacking || 
            (enemyCombat != null && enemyCombat.IsAttackFinished()))
        {
            CheckForPlayer();
        }

        switch (enemyState)
        {
            case EnemyState.Idle:
                Stop();
                break;

            case EnemyState.Chasing:
                Chase();
                break;

            case EnemyState.Attacking:
                Stop();
                break;
        }
    }

    private void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void CheckForPlayer()
    {
        if (detectionPoint == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, detectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float distance = Vector2.Distance(detectionPoint.position, player.position);
            float combatDistance = Vector2.Distance(enemyCombat.attackPoint.position, player.position);

            // 공격 범위 내
            if (combatDistance <= attackRange)
            {
                if (enemyCombat != null && enemyCombat.CanAttack())
                {
                    ChangeState(EnemyState.Attacking);                    ;
                }
                // 공격 쿨다운 중에는 그냥 대기
            }
            // 추격 범위 내
            else if (distance <= detectRange)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            // 플레이어를 찾지 못함
            if (enemyState != EnemyState.Idle)
            {
                ChangeState(EnemyState.Idle);
                player = null;
            }
        }
    }

    private void Chase()
    {
        if (player == null)
        {
            Stop();
            ChangeState(EnemyState.Idle);
            return;
        }
        
        float distance = Vector2.Distance(transform.position, player.position);

        // 감지 범위를 벗어남
        if (distance > detectRange)
        {
            ChangeState(EnemyState.Idle);
            Stop();
            return;
        }
        
        // 플레이어 방향으로 이동
        Vector2 direction = (player.position - transform.position).normalized;

        // 방향에 따라 스프라이트 뒤집기
        if ((direction.x < 0 && facingDirection == 1) || 
            (direction.x > 0 && facingDirection == -1))
        {
            FlipX();
        }
        
        rb.linearVelocity = direction * movespeed;
    }

    private void FlipX()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void ChangeState(EnemyState newState)
    {
        if (enemyState != newState)
        {
            Debug.Log($"State changed: {enemyState} → {newState}");
            enemyState = newState;

            if (anim != null)
            {
                anim.SetBool("IsMoving", newState == EnemyState.Chasing);
            }
            if (enemyState == EnemyState.Attacking) Attack();
        }
    }

    private void Attack()
    {
        Debug.Log("=== Enemy Attack Triggered ===");
        if (enemyCombat != null)
        {
            enemyCombat.Attack();
        }
        else
        {
            Debug.LogError("EnemyCombat component is missing!");
        }
    }

    private void OnDrawGizmos()
    {
        if (detectionPoint != null)
        {
            // 감지 범위 (노란색)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, detectRange);
        }
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
}