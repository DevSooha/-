using UnityEngine;
using System.Collections.Generic;

public enum BulletElement
{
    Water,
    Fire,
    Lightning
}
public enum BulletType
{
    Spiral1,
    Spiral2
}
public enum PotionEffect
{
    None,
    Heal,
    Stealth,
    PlayerSpeed2X,
    EnemySpeed2X,
    EnemyStun,
    BulletSpeedDown
}
public enum BulletEffect
{
    None,
    Heal,
    Damage
}
public enum DamageTarget
{
    Player,
    Enemy,
    Both
}


[System.Serializable]
public class BulletPatternData
{
    public int damage;
    public int effectTime;
    public int bulletCount = 3;
    public float bulletSpacing = 32f;
    public float bulletSpeed = 160f;
    public float fireInterval = 4f;
    public float totalDuration = 8f;
    public BulletType bulletType;
    public BulletElement element;
    
    public DamageTarget damageTarget;
    public BulletEffect bulletEffect;
    public PotionEffect potionEffect;
}

[CreateAssetMenu(
    fileName = "NewPotionData",
    menuName = "Inventory/Potion Data",
    order = 1)]
public class PotionData : ItemData
{
    [SerializeField] private List<BulletPatternData> patterns = new List<BulletPatternData>();
    [SerializeField] public PotionEffect potionEffect;
    
    public List<BulletPatternData> GetPatterns() => patterns;
    
    
}
