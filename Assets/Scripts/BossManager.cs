using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [Header("Settings")]
    public GameObject doorObject;
    public BossAI bossAI; // 여기에 보스(BossAI)를 연결하세요!

    // 현재 보스전이 진행 중인지 여부
    public bool IsBossActive { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            Debug.Log(" GameManager(BossManager) 생성 및 고정 완료!");
        }
        else
        {
            // 이미 다른 GameManager가 있다면, 새로 생긴 나는 사라진다.
            Debug.Log(" 중복된 GameManager 삭제됨.");
            Destroy(gameObject);
        }
    }

    // 보스방 진입 시 호출 (Trigger 등에서 호출)
    public void StartBossBattle()
    {
        // 보스 공격 시작 명령!
        if (bossAI != null)
        {

            bossAI.gameObject.SetActive(true);
            bossAI.StartBattle();
        }
        
        Debug.Log("보스전 시작! 공격 개시!");
    }

    // 보스 처치 시 호출 (BossHealth에서 호출 예정)
    public void EndBossBattle()
    {
        IsBossActive = false;

        if (doorObject != null)
        {
            doorObject.SetActive(false);
        }
        Debug.Log("보스 처치! 문이 열립니다.");
    }
}