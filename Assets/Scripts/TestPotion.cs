using UnityEngine;

public class TestPotion : MonoBehaviour
{
    [SerializeField] private PotionData pastelSpark;
    [SerializeField] private PotionData helioFlare;
    [SerializeField] private Player player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pastelSpark == null)
            {
                Debug.LogError("포션 설정이 없습니다!");
                return;
            }
            
            PotionManager.instance.UsePotion(pastelSpark, player.transform.position);
            
            Debug.Log("포션 사용!");
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (helioFlare == null)
            {
                Debug.LogError("포션 설정이 없습니다!");
                return;
            }
            
            PotionManager.instance.UsePotion(helioFlare, player.transform.position);
            
            Debug.Log("포션 사용!");
        }
    }
}

