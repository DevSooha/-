using UnityEngine;

public class PotionCraft: MonoBehaviour
{
    public enum PotionType { Failure, LowTemp, MidTemp, HighTemp }

    public static PotionType DeterminePotionType(float gaugeValue)
    {
        if (gaugeValue < 25f)
            return PotionType.Failure;
        else if (gaugeValue < 50f)
            return PotionType.LowTemp;
        else if (gaugeValue < 75f)
            return PotionType.MidTemp;
        else
            return PotionType.HighTemp;
    }

    public static void CreatePotion(PotionType type)
    {
        switch(type)
        {
            case PotionType.Failure:
                Debug.Log("포션 제작 실패!");
                break;
            case PotionType.LowTemp:
                Debug.Log("저온 포션 생성!");
                break;
            case PotionType.MidTemp:
                Debug.Log("중온 포션 생성!");
                break;
            case PotionType.HighTemp:
                Debug.Log("고온 포션 생성!");
                break;
        }
    }

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
}
