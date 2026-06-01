using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockStage(int stageNumber)
    {
        if(stageNumber >
           SaveManager.Instance.data.highestUnlockedStage)
        {
            SaveManager.Instance.data
                .highestUnlockedStage = stageNumber;

            SaveManager.Instance.SaveGame();
        }
    }

    public bool IsUnlocked(int stageNumber)
    {
        return stageNumber <=
            SaveManager.Instance.data.highestUnlockedStage;
    }
}