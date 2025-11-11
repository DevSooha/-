using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int HP;
    public int maxHP;
    public TMP_Text healthBar;

    void Start()
    {
        HP = maxHP;
        UpdateHealthBar();
    }

    // 필요할 때만 호출
    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP > maxHP)
            HP = maxHP;

        if (HP < 0)
            HP = 0;

        UpdateHealthBar();

        if (HP == 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.text = "HP: " + HP + " / " + maxHP;
    }
}
