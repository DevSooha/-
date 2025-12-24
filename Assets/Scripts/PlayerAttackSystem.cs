using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; // ★ 타일맵 기능을 쓰기 위해 필수!

// 무기 타입 정의
public enum WeaponType { None, Melee, PotionBomb }

[System.Serializable]
public class WeaponSlot
{
    public WeaponType type;
}

public class PlayerAttackSystem : MonoBehaviour
{
    [Header("Settings")]
    public float tileSize = 1.0f;
    public LayerMask enemyLayer;

    [Header("Tilemaps (필수 연결!)")]
    public Tilemap floorTilemap; // ★ 바닥 타일맵을 여기에 연결하세요!

    [Header("Prefabs")]
    public GameObject bombPrefab;
    public GameObject stackMarkerPrefab;

    [Header("Weapon Slots")]
    public List<WeaponSlot> slots = new List<WeaponSlot>();

    // 내부 변수
    private Player playerMovement;
    private Animator anim;

    private Vector2 aimDirection = Vector2.down;
    private bool isCharging = false;
    private float chargeStartTime;
    private int currentStack = 0;
    private List<GameObject> activeMarkers = new List<GameObject>();

    void Start()
    {
        playerMovement = GetComponent<Player>();
        anim = GetComponent<Animator>();

        // [추가된 부분] 바닥 타일맵 자동 찾기!
        if (floorTilemap == null)
        {
            // 1. "Ground"라는 태그가 붙은 오브젝트를 찾아서 타일맵을 가져온다.
            GameObject groundObj = GameObject.FindGameObjectWithTag("Ground");

            if (groundObj != null)
            {
                floorTilemap = groundObj.GetComponent<Tilemap>();
            }
            else
            {
                // 태그로 못 찾았으면 이름으로라도 찾아본다 (예: Grid 자식의 "Floor")
                GameObject floorObj = GameObject.Find("Floor");
                if (floorObj != null) floorTilemap = floorObj.GetComponent<Tilemap>();
            }
        }

        if (slots.Count == 0)
        {
            slots.Add(new WeaponSlot { type = WeaponType.Melee });
            slots.Add(new WeaponSlot { type = WeaponType.PotionBomb });
            slots.Add(new WeaponSlot { type = WeaponType.None });
            slots.Add(new WeaponSlot { type = WeaponType.None });
        }
    }

    void Update()
    {
        UpdateAimDirection();

        if (Input.GetKeyDown(KeyCode.C)) RotateWeaponSlots();

        if (slots[0].type == WeaponType.Melee) HandleMeleeInput();
        else if (slots[0].type == WeaponType.PotionBomb) HandleBombInput();
    }

    void UpdateAimDirection()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
            if (Mathf.Abs(x) >= Mathf.Abs(y)) aimDirection = new Vector2(x > 0 ? 1 : -1, 0);
            else aimDirection = new Vector2(0, y > 0 ? 1 : -1);
        }
    }

    void HandleMeleeInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (anim != null) anim.SetTrigger("Attack");

            Vector2 attackPos = (Vector2)transform.position + (aimDirection * tileSize);
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, tileSize * 0.5f, enemyLayer);

            foreach (Collider2D hit in hits)
            {
                BossHealth boss = hit.GetComponent<BossHealth>();
                if (boss != null) boss.TakeDamage(50, ElementType.None);
            }
        }
    }

    void HandleBombInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isCharging = true;
            chargeStartTime = Time.time;
            currentStack = 0;
            if (playerMovement != null) playerMovement.SetCanMove(false);
            StartCoroutine(ChargeRoutine());
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            isCharging = false;
            StopAllCoroutines();
            if (playerMovement != null) playerMovement.SetCanMove(true);

            float duration = Time.time - chargeStartTime;

            if (duration < 0.5f) SpawnBombAt(1);
            else SpawnBombsByStack();

            ClearMarkers();
        }
    }

    IEnumerator ChargeRoutine()
    {
        while (isCharging)
        {
            float t = Time.time - chargeStartTime;
            int targetStack = 0;
            if (t >= 1.5f) targetStack = 3;
            else if (t >= 1.0f) targetStack = 2;
            else if (t >= 0.5f) targetStack = 1;

            if (targetStack > currentStack && targetStack <= 3)
            {
                // 다음 위치 미리 계산해서 막혀있으면 스택 증가 안 함
                Vector2 nextPos = (Vector2)transform.position + (aimDirection * tileSize * (currentStack + 1));

                if (!IsValidTile(nextPos))
                {
                    Debug.Log("장애물/허공 때문에 차징 중단");
                }
                else
                {
                    currentStack = targetStack;
                    ShowStackMarker(currentStack);
                }
            }
            yield return null;
        }
    }

    // ★ 수정된 핵심 함수 (타일맵 + 콜라이더 체크)
    bool IsValidTile(Vector2 pos)
    {
        // 1. [기획서 49번] 바닥 타일이 없는 경우 (허공)
        if (floorTilemap != null)
        {
            // 월드 좌표를 타일맵 좌표(Cell)로 변환
            Vector3Int cellPos = floorTilemap.WorldToCell(pos);
            // 해당 위치에 타일이 없으면 설치 불가!
            if (!floorTilemap.HasTile(cellPos)) return false;
        }

        // 2. 물리적 장애물 검사 (벽, 계단, 오브젝트)
        // 타일맵 콜라이더도 여기서 걸립니다.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos, tileSize * 0.3f);

        foreach (var col in hitColliders)
        {
            // [기획서 51번] 장애물 (Obstacle 태그)
            if (col.CompareTag("Obstacle")) return false;

            // [기획서 50번] 계단 (Stairs 태그)
            if (col.CompareTag("Stairs")) return false;
        }

        return true;
    }

    void SpawnBombAt(int distance)
    {
        Vector2 pos = (Vector2)transform.position + (aimDirection * tileSize * distance);
        if (IsValidTile(pos))
        {
            if (bombPrefab != null) Instantiate(bombPrefab, pos, Quaternion.identity);
        }
    }

    void SpawnBombsByStack()
    {
        for (int i = 1; i <= currentStack; i++)
        {
            Vector2 pos = (Vector2)transform.position + (aimDirection * tileSize * i);

            // 중간에 막히면 뒤쪽도 설치 안 함 (관통 방지)
            if (!IsValidTile(pos)) break;

            if (bombPrefab != null) Instantiate(bombPrefab, pos, Quaternion.identity);
        }
    }

    void ShowStackMarker(int index)
    {
        Vector2 pos = (Vector2)transform.position + (aimDirection * tileSize * index);
        if (!IsValidTile(pos)) return;

        if (stackMarkerPrefab != null)
        {
            GameObject marker = Instantiate(stackMarkerPrefab, pos, Quaternion.identity);
            activeMarkers.Add(marker);
        }
    }

    void ClearMarkers()
    {
        foreach (var m in activeMarkers) if (m) Destroy(m);
        activeMarkers.Clear();
    }

    void RotateWeaponSlots()
    {
        List<WeaponSlot> valid = new List<WeaponSlot>();
        foreach (var s in slots) if (s.type != WeaponType.None) valid.Add(s);
        if (valid.Count <= 1) return;
        WeaponSlot first = valid[0];
        valid.RemoveAt(0);
        valid.Add(first);
        for (int i = 0; i < 4; i++)
        {
            if (i < valid.Count) slots[i] = valid[i];
            else slots[i] = new WeaponSlot { type = WeaponType.None };
        }
        Debug.Log($"무기 교체됨: {slots[0].type}");
    }
}