using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum WeaponType { None, Melee, PotionBomb }

[System.Serializable]
public class WeaponSlot
{
    public WeaponType type;
<<<<<<< HEAD
    //public ItemData itemData; // 연동을 위해 데이터 추가
    public GameObject specificPrefab; // 던질 물체 프리팹

    // -1이면 무제한 (근접무기), 양수면 소모품
    public int count = -1;
=======
    public GameObject specificPrefab; // ★ 이 슬롯이 사용할 전용 폭탄 프리팹
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
}

public class PlayerAttackSystem : MonoBehaviour
{
    [Header("Settings")]
    public float tileSize = 1.0f;
    public LayerMask enemyLayer;

    [Header("Tilemaps")]
    public Tilemap floorTilemap;

<<<<<<< HEAD
    [Header("Prefabs")]
    public GameObject defaultBombPrefab; // 기본 폭탄 프리팹
=======
    [Header("Prefabs (인스펙터에서 넣어주세요)")]
    // ★ 종류별 폭탄 프리팹을 여기에 등록해둡니다.
    public GameObject bombPrefab1; // 빨간 폭탄?
    public GameObject bombPrefab2; // 파란 폭탄?
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    public GameObject stackMarkerPrefab;

    [Header("Weapon Slots")]
    public List<WeaponSlot> slots = new List<WeaponSlot>();

<<<<<<< HEAD
    // 컴포넌트 캐싱
=======
    // 내부 변수
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    private Player playerMovement;
    private Animator anim;

    private Vector2 aimDirection = Vector2.down;
<<<<<<< HEAD

    // 상태 변수
    private bool isAttack = false;
    private bool isCharging = false;

=======
    private bool isCharging = false;
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    private float chargeStartTime;
    private int currentStack = 0;
    private List<GameObject> activeMarkers = new List<GameObject>();

<<<<<<< HEAD
    private PlayerInteraction interactionSensor;

=======
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    void Start()
    {
        playerMovement = GetComponent<Player>();
        anim = GetComponent<Animator>();

<<<<<<< HEAD
        // 타일맵 자동 찾기
=======
        // 타일맵 자동 찾기 (기존 코드 유지)
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
        if (floorTilemap == null)
        {
            GameObject groundObj = GameObject.FindGameObjectWithTag("Ground");
            if (groundObj != null) floorTilemap = groundObj.GetComponent<Tilemap>();
            else
            {
                GameObject floorObj = GameObject.Find("Floor");
                if (floorObj != null) floorTilemap = floorObj.GetComponent<Tilemap>();
            }
        }

<<<<<<< HEAD
        // 초기 슬롯이 비어있으면 기본값 세팅 (테스트용)
        if (slots.Count == 0)
        {
            slots.Add(new WeaponSlot { type = WeaponType.Melee, count = -1 });
        }

        interactionSensor = GetComponentInChildren<PlayerInteraction>();
=======
        // ★ 슬롯 초기화 부분 수정
        // 슬롯마다 서로 다른 폭탄 프리팹을 넣어줍니다.
        if (slots.Count == 0)
        {
            // 0번 슬롯: 근접 무기
            slots.Add(new WeaponSlot { type = WeaponType.Melee });

            // 1번 슬롯: 폭탄 1번 (예: 일반 폭탄)
            slots.Add(new WeaponSlot { type = WeaponType.PotionBomb, specificPrefab = bombPrefab1 });

            // 2번 슬롯: 폭탄 2번 (예: 얼음 폭탄) - 테스트를 위해 추가해봄
            slots.Add(new WeaponSlot { type = WeaponType.PotionBomb, specificPrefab = bombPrefab2 });

            // 3번 슬롯: 비움
            slots.Add(new WeaponSlot { type = WeaponType.None });
        }
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    }

    void Update()
    {
        UpdateAimDirection();

<<<<<<< HEAD
        // NPC 대화 중이면 공격 불가
        if (interactionSensor != null && interactionSensor.IsInteractable)
        {
            return;
        }

        // 무기 교체 (C키)
        if (!isAttack && !isCharging && Input.GetKeyDown(KeyCode.C))
        {
            RotateWeaponSlots();
        }

        // 현재 슬롯의 무기 사용
        if (slots.Count > 0 && slots[0].type != WeaponType.None)
        {
            if (slots[0].type == WeaponType.Melee)
            {
                if (!isAttack) HandleMeleeInput();
            }
            else if (slots[0].type == WeaponType.PotionBomb)
            {
                HandleBombInput();
            }
        }
    }

    // [추가] 인벤토리에서 포션을 장착하는 함수
    //public void EquipPotionFromInventory(Item item)
    //{
    //    if (item == null || item.data == null) return;

    //    // 1. 새 무기 슬롯 생성
    //    WeaponSlot newSlot = new WeaponSlot();
    //    newSlot.type = WeaponType.PotionBomb;
    //    newSlot.itemData = item.data;
    //    newSlot.count = item.quantity; // 현재 개수 반영

    //    // 프리팹 설정 (ItemData에 프리팹이 있다고 가정하거나 기본값 사용)
    //    // 만약 ItemData에 던지는 프리팹이 없다면 defaultBombPrefab 사용
    //    newSlot.specificPrefab = defaultBombPrefab;

    //    // 2. 현재 슬롯(0번)을 교체 (또는 목록에 추가)
    //    // 여기서는 "0번 슬롯을 덮어씌우는 방식"으로 구현
    //    if (slots.Count > 0)
    //    {
    //        slots[0] = newSlot;
    //    }
    //    else
    //    {
    //        slots.Add(newSlot);
    //    }

    //    Debug.Log($"무기 장착: {item.data.name} ({item.quantity}개)");
    //}

=======
        if (Input.GetKeyDown(KeyCode.C)) RotateWeaponSlots();

        // 현재(0번) 슬롯의 타입에 따라 행동 결정
        if (slots[0].type == WeaponType.Melee) HandleMeleeInput();
        else if (slots[0].type == WeaponType.PotionBomb) HandleBombInput();
    }

>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    void UpdateAimDirection()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
<<<<<<< HEAD
            // .normalized를 붙여 대각선일 때 길이가 1보다 커지는 것을 방지
            aimDirection = new Vector2(x, y).normalized;
=======
            if (Mathf.Abs(x) >= Mathf.Abs(y)) aimDirection = new Vector2(x > 0 ? 1 : -1, 0);
            else aimDirection = new Vector2(0, y > 0 ? 1 : -1);
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
        }
    }

    void HandleMeleeInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
<<<<<<< HEAD
            StartCoroutine(MeleeAttackRoutine());
        }
    }

    IEnumerator MeleeAttackRoutine()
    {
        isAttack = true;
        if (anim != null) anim.SetTrigger("IsAttack");
        yield return null;
        if (anim != null) anim.ResetTrigger("IsAttack");

        Vector2 attackPos = (Vector2)transform.position + (aimDirection * tileSize);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, tileSize * 0.7f, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            // 적 피격 처리
            EnemyCombat enemy = hit.GetComponent<EnemyCombat>();
            if (enemy != null)
            {
                enemy.EnemyTakeDamage(10);
                continue;
            }

            BossHealth boss = hit.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(50, ElementType.None);
            }
        }

        yield return new WaitForSeconds(0.4f);
        isAttack = false;
=======
            if (anim != null) anim.SetTrigger("Attack");

            Vector2 attackPos = (Vector2)transform.position + (aimDirection * tileSize);
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, tileSize * 0.5f, enemyLayer);

            foreach (Collider2D hit in hits)
            {
                BossHealth boss = hit.GetComponent<BossHealth>();
                if (boss != null) boss.TakeDamage(50, ElementType.None);
            }
        }
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    }

    void HandleBombInput()
    {
<<<<<<< HEAD
        // 소지 개수 체크
        if (slots[0].count == 0)
        {
            Debug.Log("포션이 다 떨어졌습니다!");
            return;
        }

=======
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
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

<<<<<<< HEAD
            // 짧게 누르면 1칸, 길게 누르면 스택만큼
=======
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
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

<<<<<<< HEAD
            // 소지 개수보다 많이 던질 순 없음
            if (slots[0].count != -1 && targetStack > slots[0].count)
            {
                targetStack = slots[0].count;
            }

=======
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
            if (targetStack > currentStack && targetStack <= 3)
            {
                Vector2 nextPos = (Vector2)transform.position + (aimDirection * tileSize * (currentStack + 1));

<<<<<<< HEAD
                if (IsValidTile(nextPos))
=======
                if (!IsValidTile(nextPos))
                {
                    Debug.Log("설치 불가 지역");
                }
                else
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
                {
                    currentStack = targetStack;
                    ShowStackMarker(currentStack);
                }
            }
            yield return null;
        }
    }

    bool IsValidTile(Vector2 pos)
    {
        if (floorTilemap != null)
        {
            Vector3Int cellPos = floorTilemap.WorldToCell(pos);
<<<<<<< HEAD
            // 타일이 존재해야 던질 수 있음 (벽이나 허공 방지)
            return floorTilemap.HasTile(cellPos);
        }
        return true;
    }

    void ShowStackMarker(int stackIndex)
    {
        if (stackMarkerPrefab == null) return;

        Vector2 spawnPos = (Vector2)transform.position + (aimDirection * tileSize * stackIndex);
        GameObject marker = Instantiate(stackMarkerPrefab, spawnPos, Quaternion.identity);
        activeMarkers.Add(marker);
    }

    void ClearMarkers()
    {
        foreach (GameObject marker in activeMarkers)
        {
            if (marker != null) Destroy(marker);
        }
        activeMarkers.Clear();
=======
            if (!floorTilemap.HasTile(cellPos)) return false;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos, tileSize * 0.3f);
        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Obstacle")) return false;
            if (col.CompareTag("Stairs")) return false;
        }

        return true;
    }

    // ★ 현재 장착된 슬롯(slots[0])의 프리팹을 가져오는 헬퍼 함수
    GameObject GetCurrentBombPrefab()
    {
        if (slots.Count > 0 && slots[0].specificPrefab != null)
        {
            return slots[0].specificPrefab;
        }
        return null; // 프리팹이 없으면 null 반환
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    }

    void SpawnBombAt(int distance)
    {
<<<<<<< HEAD
        if (slots.Count == 0) return;

        Vector2 spawnPos = (Vector2)transform.position + (aimDirection * tileSize * distance);

        if (!IsValidTile(spawnPos)) return;

        // 1. 프리팹 생성 (인스펙터에 등록된 defaultBombPrefab 혹은 슬롯별 프리팹)
        GameObject prefabToUse = slots[0].specificPrefab != null ? slots[0].specificPrefab : defaultBombPrefab;

        if (prefabToUse != null)
        {
            GameObject bombObj = Instantiate(prefabToUse, spawnPos, Quaternion.identity);

            // 2. 생성된 오브젝트에서 Bomb 컴포넌트 가져오기
            //Bomb bombScript = bombObj.GetComponent<Bomb>();

            // 3. ★ 핵심: PotionData 주입 (이 부분이 있어야 새로운 로직이 작동합니다)
            //if (bombScript != null)
            //{
            //// 현재 슬롯의 itemData를 PotionData로 형변환하여 주입
            //if (slots[0].itemData is PotionData pData)
            //{
            //    bombScript.Initialize(pData);
            //}
            //else
            //{
            //    Debug.LogError("현재 슬롯의 아이템이 PotionData 형식이 아닙니다!");
            //}
            //}

            UseAmmo(1);
=======
        Vector2 pos = (Vector2)transform.position + (aimDirection * tileSize * distance);

        // ★ 현재 슬롯에 등록된 폭탄 프리팹 가져오기
        GameObject bombToSpawn = GetCurrentBombPrefab();

        if (IsValidTile(pos) && bombToSpawn != null)
        {
            Instantiate(bombToSpawn, pos, Quaternion.identity);
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
        }
    }

    void SpawnBombsByStack()
    {
<<<<<<< HEAD
        if (currentStack == 0)
        {
            SpawnBombAt(1);
            return;
        }

        // 스택 1, 2, 3 위치에 순차적으로 생성
        for (int i = 1; i <= currentStack; i++)
        {
            SpawnBombAt(i);
        }
    }

    // 탄약 소모 처리
    void UseAmmo(int amount)
    {
        if (slots[0].count == -1) return; // 무한 탄창

        slots[0].count -= amount;

        // 인벤토리 데이터와 동기화 (선택 사항)
        //if (Inventory.Instance != null && slots[0].itemData != null)
        //{
        //    // 인벤토리에서도 개수를 줄이려면 Inventory에 RemoveItem 로직이 필요함
        //    // 현재는 PlayerAttackSystem 슬롯 상에서만 줄어듬
        //}

        if (slots[0].count <= 0)
        {
            slots[0].count = 0;
            // 다 쓰면 맨손(Melee)으로 전환할지 여부 결정
            // slots[0].type = WeaponType.Melee;
        }
    }

    void RotateWeaponSlots()
    {
        if (slots.Count <= 1) return;

        WeaponSlot first = slots[0];
        slots.RemoveAt(0);
        slots.Add(first);
        Debug.Log($"무기 교체됨: {slots[0].type}");
=======
        // ★ 현재 슬롯에 등록된 폭탄 프리팹 가져오기
        GameObject bombToSpawn = GetCurrentBombPrefab();

        if (bombToSpawn == null) return;

        for (int i = 1; i <= currentStack; i++)
        {
            Vector2 pos = (Vector2)transform.position + (aimDirection * tileSize * i);

            if (!IsValidTile(pos)) break;

            Instantiate(bombToSpawn, pos, Quaternion.identity);
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
        // 빈 슬롯은 제외하고 회전
        List<WeaponSlot> valid = new List<WeaponSlot>();
        foreach (var s in slots) if (s.type != WeaponType.None) valid.Add(s);

        if (valid.Count <= 1) return;

        WeaponSlot first = valid[0];
        valid.RemoveAt(0);
        valid.Add(first);

        // 다시 슬롯 배열에 적용
        for (int i = 0; i < 4; i++)
        {
            if (i < valid.Count) slots[i] = valid[i];
            else slots[i] = new WeaponSlot { type = WeaponType.None };
        }

        Debug.Log($"무기 교체됨: {slots[0].type} / 프리팹: {slots[0].specificPrefab?.name}");
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    }
}