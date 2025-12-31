using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Cinemachine을 안 쓴다면 이 줄은 없어도 됩니다. (에러나면 지우세요)
// using Cinemachine; 

public class Player : MonoBehaviour
{
    // ★ 전 세계에 단 하나뿐인 플레이어 (싱글톤)
    public static Player Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    // 버프 관련 변수
    public float baseSpeed = 5f;
    float buffTimeLeft = 0f;

    private bool knockedBack = false;
    private bool canMove = true;

    // 위치 저장 변수
    private Vector3 savedPosition;
    private bool hasSavedPosition = false;

    [Header("Animation Settings")]
    private Vector2 lastDirection;
    private float attackDirection = 1f;
    private bool isAttacking = false;

    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    public Animator animator;

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        moveSpeed = baseSpeed;
    }

    void Update()
    {
        // 1. 넉백 및 이동 불가 상태 체크
        if (knockedBack) return;

        if (isAttacking)
        {
            moveInput = Vector2.zero;
            return;
        }

        if (!canMove)
        {
            moveInput = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        // 2. 이동 입력 처리
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical).normalized;
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        // 3. 애니메이션 처리
        if (isMoving)
        {
            lastDirection = moveInput;

            if (horizontal != 0)
            {
                attackDirection = horizontal > 0 ? 1f : -1f;
                animator.SetFloat("AttackDir", attackDirection);
            }

            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
        else
        {
            animator.SetFloat("InputX", lastDirection.x);
            animator.SetFloat("InputY", lastDirection.y);
        }

        animator.SetBool("IsMoving", isMoving);

        // 4. 공격 입력
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PerformAttack());
        }

        // 버프 시간 체크
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
        if (knockedBack) return;

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }

    // 버프 적용 함수
    public void ApplySpeedBuff(float duration)
    {
        buffTimeLeft = duration;
        moveSpeed = baseSpeed * 2;
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        animator.SetTrigger("IsAttack");
        yield return null;
        animator.ResetTrigger("IsAttack");
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
        }
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
        StartCoroutine(KnockBackCounter(stunTime));
    }

    IEnumerator KnockBackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        knockedBack = false;
    }

    // 아이템 주울 때 공격 모션 캔슬
    public void CancelAttack()
    {
        StopCoroutine("PerformAttack");
        isAttacking = false;
        animator.ResetTrigger("IsAttack");
    }

    // --- 씬 이동 및 위치 저장 관련 (중복 제거 및 정리됨) ---

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드 시 위치 복구 및 카메라 연결
    // 1. 이 함수를 통째로 교체하세요
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Field" || scene.name == "MainScene")
        {
            if (hasSavedPosition)
            {
                transform.position = savedPosition;
            }

            SetCanMove(true);

            // ★ 바로 옮기지 말고, 0.1초만 기다렸다가 옮깁니다! (이게 핵심)
            StartCoroutine(ForceCameraSync());
        }
    }

    // Player.cs

    IEnumerator ForceCameraSync()
    {
        // 1. 다른 스크립트들이 초기화될 때까지 충분히 기다림
        yield return null;
        yield return new WaitForEndOfFrame();

        // 2. 룸매니저 찾기
        RoomManager roomManager = FindFirstObjectByType<RoomManager>();

        if (roomManager != null)
        {
            // ★★★ "룸매니저야, 내가 있는 방으로 카메라 좀 맞춰줘!" ★★★
            roomManager.SyncCameraToPlayer();
        }
        else
        {
            // 룸매니저가 없으면(테스트 씬 등) 그냥 내가 카메라 옮김
            if (Camera.main != null)
            {
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
            }
        }

        // 3. 시네머신 쓴다면 여기서 갱신 (안 쓰면 무시)
        /*
        var vCam = FindFirstObjectByType<Cinemachine.CinemachineVirtualCamera>();
        if (vCam != null)
        {
            vCam.Follow = transform;
            vCam.OnTargetObjectWarped(transform, transform.position - vCam.transform.position);
        }
        */
    }

    // ★ 위치 저장 함수 (클래스 안으로 들어옴)
    public void SaveCurrentPosition()
    {
        savedPosition = transform.position;
        hasSavedPosition = true;
        Debug.Log($"좌표 저장됨: {savedPosition}");
    }

} // Player 클래스 끝