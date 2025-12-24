using UnityEngine;
using TMPro;

public class MapNode : MonoBehaviour
{
    public enum Direction { North, South, East, West }

    [Header("방 이동 설정")]
    public Direction moveDirection;
    public RoomData nextRoom;

    [Header("이동 거리 개별 설정 (0이면 기본값 사용)")]
    // 이 문을 통과할 때 얼마만큼 카메라를 이동시킬지 설정
    public float overrideDistance = 0f;

    [Header("차단 메시지 설정")]
    public string defaultBlockMessage = "길이 막혀 있습니다.";
    public string lockedMessage = "보스전 중에는 이동할 수 없습니다.";

    private BoxCollider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();

        // 연결된 방이 없으면 벽(False), 있으면 문(True)
        if (myCollider != null)
        {
            if (nextRoom == null) myCollider.isTrigger = false;
            else myCollider.isTrigger = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && nextRoom == null)
        {
            ShowMessage(defaultBlockMessage);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryEnterRoom(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryEnterRoom(other);
    }

    private void TryEnterRoom(Collider2D other)
    {
        if (other.CompareTag("Player") && other is CapsuleCollider2D)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            if (!IsPushingTowardsDoor(inputX, inputY)) return;

            if (nextRoom == null)
            {
                ShowMessage(defaultBlockMessage);
                return;
            }

            if (BossManager.Instance != null && BossManager.Instance.IsBossActive)
            {
                ShowMessage(lockedMessage);
                PushBack(other.gameObject);
                return;
            }

            // [핵심 변경] 이동 요청 시, 이 문에 설정된 '거리(overrideDistance)'도 같이 보냄
            Vector2 dirVector = GetDirectionVector();
            RoomManager.Instance.RequestMove(dirVector, nextRoom, overrideDistance);
        }
    }

    private bool IsPushingTowardsDoor(float x, float y)
    {
        switch (moveDirection)
        {
            case Direction.North: return y > 0.5f;
            case Direction.South: return y < -0.5f;
            case Direction.East: return x > 0.5f;
            case Direction.West: return x < -0.5f;
            default: return false;
        }
    }

    private void ShowMessage(string message)
    {
        if (UIManager.Instance != null) UIManager.Instance.ShowWarning(message);
    }

    private void PushBack(GameObject player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 pushDirection = (player.transform.position - transform.position).normalized;
            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(pushDirection * 10f, ForceMode2D.Impulse);
        }
    }

    private Vector2 GetDirectionVector()
    {
        switch (moveDirection)
        {
            case Direction.North: return Vector2.up;
            case Direction.South: return Vector2.down;
            case Direction.East: return Vector2.right;
            case Direction.West: return Vector2.left;
            default: return Vector2.zero;
        }
    }
}