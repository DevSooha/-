using UnityEngine;


public enum BulletElement
{
    Water,
    Fire,
    Lightning
}
public enum BulletType
{
    Fireworks,
    Bomb,
    Spiral
}
public enum PotionEffect
{
    None,
    HealingBullets,
    Stealth,
    PlayerSpeed2X,
    EnemySpeed2X,
    EnemyStun,
    BulletSpeedDown
}
public enum DamageTarget
{
    Player,
    Enemy,
    Both
}

[CreateAssetMenu(
    fileName = "NewPotionData",
    menuName = "Inventory/Potion Data",
    order = 1)]

public class PotionData : ItemData
{
    public int damage1;
    public int damage2;
    public int effectTime;
    [SerializeField] public BulletType bulletType1;
    [SerializeField] public BulletType bulletType2;
    [SerializeField] public PotionEffect potionEffect1;
    [SerializeField] public PotionEffect potionEffect2;
    [SerializeField] public DamageTarget damageTarget1;
    [SerializeField] public DamageTarget damageTarget2;
    [SerializeField] public BulletElement element1;
    [SerializeField] public BulletElement element2;
}
