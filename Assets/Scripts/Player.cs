using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    public float baseSpeed = 5f;    
    float buffTimeLeft = 0f;
    public float facingDirection = -1;
    private bool knockedBack = false;
    private bool canMove = true;

    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        if (knockedBack == false)
        {
            if (!canMove)
            {
                moveInput = Vector2.zero;
                return;
            }
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            moveInput = new Vector2(horizontal, vertical).normalized;
            bool isMoving = moveInput.magnitude > 0;
            if ((horizontal < 0 && transform.localScale.x < 0) || (horizontal>0 && transform.localScale.x>0))
            {
                FlipX();
            }
        }
         if (buffTimeLeft > 0f)
    {
        buffTimeLeft -= Time.deltaTime;
        if (buffTimeLeft <= 0f)
        {
            buffTimeLeft = 0f;
            moveSpeed = baseSpeed;   
        }
    }
    }

    void FixedUpdate()
    {
        if (!knockedBack)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    void FlipX()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    public bool CanMove()
    {
        return canMove;
    }
    public void KnockBack(Transform enemy, float force, float stunTime)
    {
        knockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockBackCouter(stunTime));     
    }
    IEnumerator KnockBackCouter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        knockedBack = false;
    }
    public void ApplySpeedBuff(float duration)
{
    buffTimeLeft = duration;
    moveSpeed = baseSpeed * 2;
}
}