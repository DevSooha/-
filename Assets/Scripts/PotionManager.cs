using UnityEngine;

public class PotionManager : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject bulletPatternPrefab;
    [SerializeField] private GameObject bulletPrefabBlue;
    [SerializeField] private GameObject bulletPrefabRed;
    [SerializeField] private GameObject bulletPrefabGreen;

    // Inspector에서 연결 안 해도 됨 (코드로 찾음)
    [SerializeField] private Player player;

    // ★ 삭제: 이 변수는 값이 할당된 적이 없어서 에러의 원인입니다.
    // private BulletPatternData patternData; 

    public static PotionManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // ★ 추가됨: 게임 시작 시 플레이어를 스스로 찾습니다.
    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    public void UsePotion(PotionData potionData, Vector3 position)
    {
        if (potionData == null)
        {
            Debug.LogError("포션 설정이 없습니다!");
            return;
        }

        // ★ 수정됨: 멤버변수 patternData 대신, 매개변수 potionData의 정보를 바로 씁니다.
        // (PotionData 안에 effectTime과 potionEffect가 있다고 가정)
        // 만약 PotionData 구조가 다르다면 potionData.patternData.effectTime 등으로 바꾸세요.
        ApplyBuff(potionData);

        GameObject explosionObj = Instantiate(explosionPrefab, position, Quaternion.identity);
        PotionExplosion explosion = explosionObj.GetComponent<PotionExplosion>();

        if (explosion == null)
            explosion = explosionObj.AddComponent<PotionExplosion>();

        // PotionData config = potionData as PotionData; // (불필요한 형변환 삭제)

        explosion.Initialize(potionData, bulletPatternPrefab, bulletPrefabBlue, bulletPrefabRed, bulletPrefabGreen, position);
    }

    // ★ 수정됨: float duration만 받지 않고 데이터 전체를 받아서 처리
    public void ApplyBuff(PotionData data)
    {
        // ★ 중요: 플레이어가 없으면(크래프팅 씬 등) 버프 주지 말고 리턴
        if (player == null) return;

        // ★ patternData 대신 매개변수로 받은 data 사용
        switch (data.potionEffect)
        {
            case PotionEffect.PlayerSpeed2X:
                // duration도 데이터에서 직접 가져옴
                player.ApplySpeedBuff(data.effectTime);
                break;
            case PotionEffect.None:
                break;
            case PotionEffect.Heal:
                // 예시: player.Heal(data.amount);
                break;
            case PotionEffect.EnemySpeed2X:
                break;
            case PotionEffect.EnemyStun:
                break;
            case PotionEffect.BulletSpeedDown:
                break;
        }
    }
}