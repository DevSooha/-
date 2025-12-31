using UnityEngine;
using System.Collections;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;
    public int quantity = 1;

    private bool initialized = false;
    private bool isPickingUp = false;
    private bool isPlayerInRange = false; // 플레이어가 근처에 있나?
    private Transform playerTransform;    // 플레이어 위치 기억용

    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void Init(ItemData data, int amount)
    {
        itemData = data;
        quantity = amount;
        initialized = true;
        col.enabled = true;
    }

    private void Update()
    {
        // 1. 이미 줍는 중이면 패스
        if (isPickingUp) return;

        // 2. 초기화 안 됐으면 패스
        if (!initialized) return;

        // 3. ★ 핵심: 플레이어가 범위 안에 있고 + Z키를 눌렀을 때 ★
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            TryPickup();
        }
    }

    // 플레이어가 범위 안에 들어옴
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;
        }
    }

    // 플레이어가 범위 밖으로 나감
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;
        }
    }

    void TryPickup()
    {
        // 1. 아이템 데이터 확인
        if (itemData == null)
        {
            Debug.LogError($"[오류] {gameObject.name} 아이템에 'Item Data'가 비어있습니다! 인스펙터 확인하세요.");
            return;
        }

        // 2. 인벤토리 연결 확인
        if (Inventory.Instance == null)
        {
            Debug.LogError("[오류] 씬에 'GameManager(Inventory)'가 없습니다! 아이템을 넣을 곳이 없어요.");
            return;
        }

        if (Player.Instance != null)
        {
            Player.Instance.CancelAttack();
        }

        // 3. 문제 없으면 줍기 시작
        StartCoroutine(PickupEffect());
    }

    IEnumerator PickupEffect()
    {
        isPickingUp = true;

        // ★ 인벤토리에 아이템 추가
        Inventory.Instance.AddItem(itemData, quantity);
        Debug.Log($"아이템 획득: {itemData.name} ({quantity}개)");

        // --- 날아가는 연출 ---
        float time = 0f;
        float duration = 0.3f;
        Vector3 start = transform.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // 플레이어가 있으면 거기로 날아감 (없으면 제자리에서 사라짐)
            Vector3 targetPos = (playerTransform != null) ? playerTransform.position : start;

            transform.position = Vector3.Lerp(start, targetPos, t);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}