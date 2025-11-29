using UnityEngine;

public class PotionManager : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject bulletPatternPrefab;
    [SerializeField] private GameObject bulletPrefabBlue;
    [SerializeField] private GameObject bulletPrefabRed;
    [SerializeField] private GameObject bulletPrefabGreen;
    [SerializeField] private Player player;
    private BulletPatternData patternData;

    public static PotionManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void UsePotion(PotionData potionData, Vector3 position)
    {
        if (potionData == null)
        {
            Debug.LogError("포션 설정이 없습니다!");
            return;
        }
        ApplyBuff(patternData.effectTime);
        GameObject explosionObj = Instantiate(explosionPrefab, position, Quaternion.identity);
        PotionExplosion explosion = explosionObj.GetComponent<PotionExplosion>();
        
        if (explosion == null)
            explosion = explosionObj.AddComponent<PotionExplosion>();
        
        PotionData config = potionData as PotionData;
        if (potionData != null)
        {
            explosion.Initialize(config, bulletPatternPrefab, bulletPrefabBlue, bulletPrefabRed, bulletPrefabGreen, position);
        }
        
    }
    public void ApplyBuff(float duration)
    {
        switch (patternData.potionEffect)
        {
            case PotionEffect.PlayerSpeed2X:
            player.ApplySpeedBuff(duration);
            break;
            case PotionEffect.None:
            break;
            case PotionEffect.Heal:
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
