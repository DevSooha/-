using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Settings")]
    public float transitionTime = 0.4f;
    public float playerSpawnOffset = 1.5f;

    [Header("1. Grid Spacing (방 배치 간격/카메라 이동 거리)")]
    [Tooltip("방과 방 사이의 거리입니다. 화면 크기보다 크게 잡으면 빈 공간(Gap)이 생깁니다.")]
    public float gridWidth = 32.0f;  // 32(화면) + 4(여백)
    public float gridHeight = 18.0f; // 18(화면) + 4(여백)

    [Header("2. Playable Size (실제 플레이 공간 크기)")]
    [Tooltip("플레이어가 밟을 수 있는 방의 실제 크기입니다. 플레이어 위치 잡을 때 씁니다.")]
    public float playableWidth = 28.0f; // UI(2+2)를 뺀 크기
    public float playableHeight = 18.0f;

    [Header("References")]
    public Camera mainCamera;
    public Transform player;

    private RoomData currentRoomData;
    private Dictionary<string, GameObject> loadedRooms = new Dictionary<string, GameObject>();
    private bool isCoolingDown = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializeFirstRoom(RoomData startRoom, Vector3 position)
    {
        currentRoomData = startRoom;
        SpawnRoom(startRoom, position);
        UpdateNeighborPreload(startRoom);

        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(position.x, position.y, -10f);
        }
        StartCoroutine(StartSpawnProtection());
    }

    IEnumerator StartSpawnProtection()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(0.5f);
        isCoolingDown = false;
    }

    public void RequestMove(Vector2 direction, RoomData nextRoom, float distanceOverride = 0f)
    {
        if (BossManager.Instance != null && BossManager.Instance.IsBossActive) return;
        if (isCoolingDown) return;
        if (!loadedRooms.ContainsKey(nextRoom.roomID))
        {
            Debug.LogWarning($"로딩 안 됨: {nextRoom.roomID}");
            return;
        }

        StartCoroutine(TransitionRoutine(direction, nextRoom, distanceOverride));
    }

    private IEnumerator TransitionRoutine(Vector2 direction, RoomData nextRoom, float distanceOverride)
    {
        isCoolingDown = true;
        SetPlayerInput(false);

        Vector3 startCameraPos = mainCamera.transform.position;
        Vector3 startPlayerPos = player.position;

        // 1. 카메라 목표 계산
        float moveDistance = 0f;
        if (direction.x != 0)
            moveDistance = (distanceOverride > 0) ? distanceOverride : gridWidth;
        else
            moveDistance = (distanceOverride > 0) ? distanceOverride : gridHeight;

        Vector3 moveAmount = new Vector3(direction.x * moveDistance, direction.y * moveDistance, 0);
        Vector3 targetCameraPos = startCameraPos + moveAmount;

        // 2. [핵심 변경] 플레이어 목표를 '정확한 착지 지점'으로 미리 계산!
        Vector3 targetPlayerPos = GetTargetPosition(direction, targetCameraPos);

        // 3. 이동 (이제 목표점이 똑같으므로 마지막에 튀지 않음)
        float elapsed = 0;
        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            // t = t * t * (3f - 2f * t); // (원하시면 부드러운 곡선 적용)

            mainCamera.transform.position = Vector3.Lerp(startCameraPos, targetCameraPos, t);
            player.position = Vector3.Lerp(startPlayerPos, targetPlayerPos, t); // 정확한 위치로 이동

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. 도착 확정
        mainCamera.transform.position = targetCameraPos;
        player.position = targetPlayerPos; // 이미 거기에 도착해있어서 튀지 않음!

        // 물리 속도 제거 (미끄러짐 방지)
        if (player.TryGetComponent<Rigidbody2D>(out var rb)) rb.linearVelocity = Vector2.zero;

        // 5. 데이터 갱신 및 엔딩 체크 (기존과 동일)
        currentRoomData = nextRoom;
        UpdateNeighborPreload(nextRoom);

        SetPlayerInput(true);

        // --- 엔딩 체크 ---
        if (loadedRooms.ContainsKey(nextRoom.roomID))
        {
            GameObject roomObj = loadedRooms[nextRoom.roomID];
            // [중요] includeInactive를 true로 해야 꺼져있는 오브젝트도 찾음
            EndingEvent ending = roomObj.GetComponentInChildren<EndingEvent>(true);

            if (ending != null)
            {
                Debug.Log("엔딩 발견! 실행합니다.");
                ending.PlayEnding();
            }
            else
            {
                Debug.Log($"엔딩 스크립트 없음. 방 이름: {roomObj.name}");
            }
        }

        yield return new WaitForSeconds(0.1f);
        isCoolingDown = false;
    }

    private void RepositionPlayer(Vector2 direction, Vector3 nextRoomCenterPos)
    {
        // Grid가 아무리 넓어도, 플레이어는 "실제 방 크기(Playable)" 끝에 서야 합니다.
        float currentHalfWidth = playableWidth / 2f;
        float currentHalfHeight = playableHeight / 2f;

        Vector3 newPos = player.position;

        if (direction == Vector2.up)
            newPos = new Vector3(player.position.x, nextRoomCenterPos.y - currentHalfHeight + playerSpawnOffset, 0);

        else if (direction == Vector2.down)
            newPos = new Vector3(player.position.x, nextRoomCenterPos.y + currentHalfHeight - playerSpawnOffset, 0);

        else if (direction == Vector2.right)
            newPos = new Vector3(nextRoomCenterPos.x - currentHalfWidth + playerSpawnOffset, player.position.y, 0);

        else if (direction == Vector2.left)
            newPos = new Vector3(nextRoomCenterPos.x + currentHalfWidth - playerSpawnOffset, player.position.y, 0);

        player.position = newPos;

        if (player.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // 플레이어가 도착해야 할 '정확한 위치'를 계산해서 반환만 하는 함수
    private Vector3 GetTargetPosition(Vector2 direction, Vector3 nextRoomCenterPos)
    {
        float currentHalfWidth = playableWidth / 2f;
        float currentHalfHeight = playableHeight / 2f;

        Vector3 targetPos = player.position;

        if (direction == Vector2.up)
            targetPos = new Vector3(player.position.x, nextRoomCenterPos.y - currentHalfHeight + playerSpawnOffset, 0);

        else if (direction == Vector2.down)
            targetPos = new Vector3(player.position.x, nextRoomCenterPos.y + currentHalfHeight - playerSpawnOffset, 0);

        else if (direction == Vector2.right)
            targetPos = new Vector3(nextRoomCenterPos.x - currentHalfWidth + playerSpawnOffset, player.position.y, 0);

        else if (direction == Vector2.left)
            targetPos = new Vector3(nextRoomCenterPos.x + currentHalfWidth - playerSpawnOffset, player.position.y, 0);

        return targetPos;
    }

    private void UpdateNeighborPreload(RoomData current)
    {
        HashSet<string> neighborsToKeep = new HashSet<string>();
        neighborsToKeep.Add(current.roomID);

        RoomData[] neighbors = { current.north, current.south, current.east, current.west };

        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
            {
                neighborsToKeep.Add(neighbor.roomID);
                if (!loadedRooms.ContainsKey(neighbor.roomID))
                {
                    // [핵심 3] 방 생성 위치는 Grid Spacing을 따릅니다. (멀찍이 떨어뜨림)
                    SpawnRoom(neighbor, CalculateRoomPosition(neighbor));
                }
            }
        }

        List<string> roomsToRemove = new List<string>();
        foreach (var loadedID in loadedRooms.Keys)
        {
            if (!neighborsToKeep.Contains(loadedID)) roomsToRemove.Add(loadedID);
        }

        foreach (var id in roomsToRemove)
        {
            Destroy(loadedRooms[id]);
            loadedRooms.Remove(id);
        }
    }

    private void SpawnRoom(RoomData data, Vector3 position)
    {
        GameObject roomObj = Instantiate(data.roomPrefab, position, Quaternion.identity);
        loadedRooms.Add(data.roomID, roomObj);
    }

    private Vector3 CalculateRoomPosition(RoomData data)
    {
        // 좌표 * 간격 (여백 포함된 넓은 간격)
        return new Vector3(data.roomCoord.x * gridWidth, data.roomCoord.y * gridHeight, 0);
    }

    private void SetPlayerInput(bool active) { }
}