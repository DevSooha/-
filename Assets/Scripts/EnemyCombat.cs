using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat")]
    public int enemyHP = 3;
    public int damageAmount = 1;

    [Header("Drop")]
    public GameObject worldItemPrefab;
    public ItemData pastelbloomItemData;   // ⭐ 기존 ItemData 사용
    public int dropAmount = 1;
    public float dropChance = 1f;

    public void EnemyTakeDamage(int amount)
    {
        enemyHP -= amount;

        if (enemyHP <= 0)
        {
            DropItem();
            Destroy(gameObject);
        }
    }

    void DropItem()
    {
        if (Random.value > dropChance) return;

        Vector3 dropPos =
            transform.position + (Vector3)Random.insideUnitCircle.normalized * 0.5f;

        GameObject item = Instantiate(
            worldItemPrefab,
            dropPos,
            Quaternion.identity
        );

        WorldItem wi = item.GetComponent<WorldItem>();

        wi.Init(pastelbloomItemData, dropAmount);

        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        sr.sprite = pastelbloomItemData.icon;
        sr.sortingLayerName = "Item";
    }

}
