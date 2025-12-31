using UnityEngine;
using UnityEngine.SceneManagement;

public class PotionCraft : MonoBehaviour
{
    // ★ 다른 스크립트가 나를 찾을 수 있게 함 (싱글톤 비슷하게)
    public static PotionCraft Instance;

    [Header("Result Potions Data (인스펙터에서 연결하세요)")]
    public PotionData lowTempPotion;
    public PotionData midTempPotion;
    public PotionData highTempPotion;
    public PotionData failureTrash;

    public enum PotionType { Failure, LowTemp, MidTemp, HighTemp }

    private void Awake()
    {
        Instance = this;
    }

    // 게이지 값으로 타입 결정
    public static PotionType DeterminePotionType(float gaugeValue)
    {
        if (gaugeValue < 25f) return PotionType.Failure;
        else if (gaugeValue < 50f) return PotionType.LowTemp;
        else if (gaugeValue < 75f) return PotionType.MidTemp;
        else return PotionType.HighTemp;
    }

    // ★ [부활] CraftUI가 찾던 그 함수입니다!
    public static void CreatePotion(PotionType type)
    {
        if (Instance == null)
        {
            Debug.LogError("PotionCraft가 씬에 없습니다!");
            return;
        }
        // 실제 처리는 인스턴스에게 넘김
        Instance.ProcessCrafting(type);
    }

    // 실제 아이템 지급 로직
    public void ProcessCrafting(PotionType type)
    {
        PotionData resultItem = null;

        switch (type)
        {
            case PotionType.Failure:
                Debug.Log("실패!");
                resultItem = failureTrash;
                break;
            case PotionType.LowTemp:
                Debug.Log("저온 포션 성공!");
                resultItem = lowTempPotion;
                break;
            case PotionType.MidTemp:
                Debug.Log("중온 포션 성공!");
                resultItem = midTempPotion;
                break;
            case PotionType.HighTemp:
                Debug.Log("고온 포션 성공!");
                resultItem = highTempPotion;
                break;
        }

        // 인벤토리에 넣기
        if (resultItem != null && Inventory.Instance != null)
        {
            Inventory.Instance.AddItem(resultItem, 1);
            Debug.Log($"인벤토리에 {resultItem.name} 들어감!");
        }
    }

    // 이름 가져오기 (UI용)
    public static string GetPotionName(PotionType type)
    {
        return type switch
        {
            PotionType.Failure => "FAILED",
            PotionType.LowTemp => "LOW TEMP POTION",
            PotionType.MidTemp => "MID TEMP POTION",
            PotionType.HighTemp => "HIGH TEMP POTION",
            _ => "Unknown"
        };
    }

    // 나가기 버튼용
    public void ExitScene()
    {
        SceneManager.LoadScene("Field"); // 돌아갈 씬 이름 확인!
    }
}