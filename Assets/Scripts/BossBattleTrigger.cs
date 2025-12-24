using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{
    private bool hasTriggered = false; // 한 번만 발동되게

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 발동됐으면 무시
        if (hasTriggered) return;

        // 플레이어가 밟았을 때
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // 보스 매니저에게 "전투 시작해!"라고 알림
            if (BossManager.Instance != null)
            {
                BossManager.Instance.StartBossBattle();
            }

            // (선택사항) 트리거 오브젝트 삭제 (더 이상 필요 없으니)
            // Destroy(gameObject); 
        }
    }
}