using UnityEngine;

public class TestGameStarter : MonoBehaviour
{
    [Header("시작 설정")]
<<<<<<< HEAD
    public RoomData startingRoom;
    public Transform player;
=======
    public RoomData startingRoom; // 가장 먼저 보여줄 방 데이터 (에셋)
    public Transform player;      // 플레이어
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6

    private void Start()
    {
        if (startingRoom == null)
        {
            Debug.LogError("GameStarter: 시작할 방 데이터가 비어있습니다!");
            return;
        }

<<<<<<< HEAD
        RoomManager.Instance.InitializeFirstRoom(startingRoom, Vector3.zero);

        if (player != null)
        {
            player.position = new Vector3(0, -5, 0);
=======
        // 1. 첫 번째 방을 (0,0,0) 좌표에 생성하고, 주변 방 프리로드 시작
        RoomManager.Instance.InitializeFirstRoom(startingRoom, Vector3.zero);

        // 2. 플레이어 위치 초기화 (방의 정중앙)
        if (player != null)
        {
            player.position = Vector3.zero;
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
        }

        Debug.Log($"게임 시작! {startingRoom.roomID} 방이 로드되었습니다.");
    }
}